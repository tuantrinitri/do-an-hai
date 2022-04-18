using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using Slugify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CMS.Areas.Identity.Models;
using Newtonsoft.Json;
using CMS.Models;
using AutoMapper;
using CMS.Helpers;
using CMS.Areas.StaticPages.DTOs;
using CMS.Areas.StaticPages.Models;

namespace CMS.Areas.StaticPages.Controllers
{
    [Area("StaticPage")]
    [Route("AdminCP/StaticPage/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    //[DefaultBreadcrumb("Danh sách")]
    public class StaticPageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SlugHelper _slugHelper = new SlugHelper();
        public StaticPageController(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Breadcrumb("Danh sách trang")]
        // GET: StaticPage/StaticPage
        public async Task<IActionResult> Index(int page = 1)
        {
            var pages = _context.StaticPages.OrderByDescending(q => q.CreatedDate);
            var pagination = PaginatedList<StaticPage>.Create(pages, page, 10);
            ViewData["totalitem"] = pages.Count();
            return View(pagination);
        }

        [Breadcrumb("ViewData.Title")]
        // GET: StaticPage/StaticPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staticPage = await _context.StaticPages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staticPage == null)
            {
                return NotFound();
            }

            return View(staticPage);
        }

        [Breadcrumb("ViewData.Title")]
        // GET: StaticPage/StaticPage/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StaticPage/StaticPage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ShortDesc,Content")] StaticPage staticPage, string submitBtn)
        {
            if (ModelState.IsValid)
            {
                SlugHelper.Config slugConfig = new SlugHelper.Config();
                slugConfig.StringReplacements.Add("đ", "d");
                SlugHelper helper = new SlugHelper(slugConfig);
                staticPage.Title = staticPage.Title.FullTrim();
                staticPage.ShortDesc = staticPage.ShortDesc.FullTrim();
                string slug = helper.GenerateSlug(staticPage.Title);
                var slugCount = await _context.StaticPages.Where(dc => dc.Slug == slug).CountAsync();
                staticPage.Slug = slugCount > 0 ? slug + "-" + Math.Round((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds) : slug;

                var name = User.Identity.Name;
                staticPage.CreatedBy = name;
                staticPage.LastModifiedBy = name;
                // var currentUser = _userManager.FindByNameAsync(name).Result;
                staticPage.CreatedDate = DateTime.UtcNow;
                staticPage.LastModifiedDate = DateTime.UtcNow;
                staticPage.Status = submitBtn;
                _context.Add(staticPage);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }

            return View(staticPage);
        }

        [Breadcrumb("ViewData.Title")]
        // GET: StaticPage/StaticPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staticPage = await _context.StaticPages.FindAsync(id);
            if (staticPage == null)
            {
                return NotFound();
            }
            UpdatePageDTO updatePageDTO = _mapper.Map<UpdatePageDTO>(staticPage);
            updatePageDTO.Slug = "/trang/" + staticPage.Slug;
            return View(updatePageDTO);
        }

        // POST: StaticPage/StaticPage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePageDTO updatePageDTO, string submitBtn)
        {
            if (id != updatePageDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    StaticPage page = _context.StaticPages.FirstOrDefault(d => d.Id == id);
                    StaticPage pageToUpdate = _mapper.Map<UpdatePageDTO, StaticPage>(updatePageDTO, page);

                    SlugHelper.Config slugConfig = new SlugHelper.Config();
                    slugConfig.StringReplacements.Add("đ", "d");
                    SlugHelper helper = new SlugHelper(slugConfig);
                    string slug = helper.GenerateSlug(pageToUpdate.Title);
                    var slugCount = await _context.StaticPages.Where(dc => dc.Slug == slug && dc.Id != page.Id).CountAsync();
                    pageToUpdate.Slug = slugCount > 0 ? slug + "-" + page.Id.ToString() : slug;

                    pageToUpdate.Title = updatePageDTO.Title.FullTrim();
                    pageToUpdate.ShortDesc = updatePageDTO.ShortDesc.FullTrim();
                    pageToUpdate.Status = submitBtn;
                    pageToUpdate.LastModifiedDate = DateTime.UtcNow;
                    pageToUpdate.LastModifiedBy = User.Identity.Name;
                    _context.Update(pageToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Cập nhật thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaticPageExists(updatePageDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return View(updatePageDTO);
        }

        // POST: StaticPage/StaticPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int? id)
        {

            var staticPage = await _context.StaticPages.FindAsync(id);
            if (staticPage == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }
            _context.StaticPages.Remove(staticPage);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        private bool StaticPageExists(int id)
        {
            return _context.StaticPages.Any(e => e.Id == id);
        }
    }
}
