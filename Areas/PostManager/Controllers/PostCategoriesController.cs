using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using AutoMapper;
using Slugify;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Helpers;
using CMS.Areas.PostManager.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Areas.PostManager.Controllers
{
    [Area("PostManager")]
    [Route("AdminCP/PostCategories/{action=index}/{id?}")]
    [Authorize]
    public class PostCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SlugHelper _slugHelper = new SlugHelper();

        public PostCategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //[Route("/")]
        // GET: PostManager/PostCategories
        [Breadcrumb("Chuyên mục")]
        public async Task<IActionResult> Index()
        {
            //return View(await _context.PostCategories.ToListAsync());
            return View(await _context.PostCategories.Where( pc => pc.IsDeleted != true).ToListAsync());
        }

        // GET: PostManager/PostCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories.Where( pc => pc.IsDeleted != true)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (postCategory == null)
            {
                return NotFound();
            }

            return View(postCategory);
        }

        // GET: PostManager/PostCategories/Create
        [Breadcrumb("Tạo mới")]
        public IActionResult Create()
        {
            return View(new CreatePostCategoryDTO { Activated = true });
        }

        // POST: PostManager/PostCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostCategoryDTO postCategoryDTO)
        {
            if (ModelState.IsValid)
            {
                PostCategory category = _mapper.Map<PostCategory>(postCategoryDTO);

                // generate slug
                SlugHelper.Config slugConfig = new SlugHelper.Config();
                slugConfig.StringReplacements.Add("đ", "d");
                SlugHelper _slugHelper = new SlugHelper(slugConfig);
                string slug = _slugHelper.GenerateSlug(category.Name.FullTrim());
                var slugCount = await _context.PostCategories.Where(p => p.Slug == slug).CountAsync();
                category.Slug = slugCount > 0 ? slug + "-" + Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) : slug;

                category.Name = postCategoryDTO.Name.FullTrim();
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            return View(postCategoryDTO);
        }

        // GET: PostManager/PostCategories/Edit/5
        [Breadcrumb("Sửa chuyên mục bài viết")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var postCategory = await _context.PostCategories.FindAsync(id);
            if (postCategory == null)
            {
                return NotFound();
            }
            UpdatePostCategoryDTO postCategoryDTO = _mapper.Map<UpdatePostCategoryDTO>(postCategory);
            return View(postCategoryDTO);
        }

        // POST: PostManager/PostCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePostCategoryDTO postCategoryDTO)
        {
            if (id != postCategoryDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    PostCategory postCategory = _context.PostCategories.FirstOrDefault(pc => pc.Id == id);
                    PostCategory postCategoryToUpdate = _mapper.Map<UpdatePostCategoryDTO, PostCategory>(postCategoryDTO, postCategory);
                    // generate slug
                    SlugHelper.Config slugConfig = new SlugHelper.Config();
                    slugConfig.StringReplacements.Add("đ", "d");
                    SlugHelper _slugHelper = new SlugHelper(slugConfig);
                    string slug = _slugHelper.GenerateSlug(postCategory.Name.FullTrim());
                    var slugCount = await _context.PostCategories.Where(p => p.Slug == slug && p.Id != postCategory.Id).CountAsync();
                    postCategoryToUpdate.Slug = slugCount > 0 ? slug + "-" + Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) : slug;

                    postCategoryToUpdate.Name = postCategoryDTO.Name.FullTrim();
                    _context.Update(postCategoryToUpdate);
                    var result = await _context.SaveChangesAsync();

                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Cập nhật dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostCategoryExists(postCategoryDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(postCategoryDTO);
        }

        // POST: PostManager/PostCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }

            var categoryToDelete = await _context.PostCategories.FindAsync(id);
            categoryToDelete.IsDeleted = true;
            _context.PostCategories.Update(categoryToDelete);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        private bool PostCategoryExists(int id)
        {
            return _context.PostCategories.Any(e => e.Id == id);
        }
    }
}
