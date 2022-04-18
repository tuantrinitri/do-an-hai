using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.HomepageManager.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs.Attributes;
using CMS.Areas.HomepageManager.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Helpers;

namespace CMS.Areas.HomepageManager.Controllers
{
    [Area("HomepageManager")]
    [Route("AdminCP/HomepageBlocks/{blockId?}/HomepageItems/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class HomepageItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HomepageItemsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: HomepageManager/HomepageItems
        [Breadcrumb("ViewData.BlockTitle", FromController = typeof(HomepageBlocksController), FromAction = "Index")]
        public async Task<IActionResult> Index(int? blockId)
        {
            var homepageItems = _context.HomepageItems.Where(l => l.BlockId == blockId).OrderBy(l => l.Order).ToList();
            int i = 1;
            foreach (var iHomepageItem in homepageItems)
            {
                iHomepageItem.Order = i;
                await _context.SaveChangesAsync();
                i++;
            }

            var block = _context.HomepageBlocks.FirstOrDefault(l => l.Id == blockId);
            if (block == null)
            {
                return RedirectToAction("Index", "HomepageBlocks");
            }

            ViewData["BlockTitle"] = block.Title;
            ViewData["Block"] = block;

            var applicationDbContext = _context.HomepageItems.Include(l => l.Block).Where(l => l.BlockId == blockId).OrderBy(l => l.Order);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HomepageManager/HomepageItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homepageItem = await _context.HomepageItems
                .Include(l => l.Block)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homepageItem == null)
            {
                return NotFound();
            }

            return View(homepageItem);
        }

        // GET: HomepageManager/HomepageItems/Create
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public IActionResult Create(int? blockId)
        {
            var block = _context.HomepageBlocks.FirstOrDefault(l => l.Id == blockId);
            if (block == null)
            {
                return RedirectToAction("Index", "HomepageBlocks");
            }

            ViewData["BlockTitle"] = block.Title;
            ViewData["Block"] = block;
            ViewBag.CategorySlug = new SelectList(_context.PostCategories, "Slug", "Name");
            return View(new CreateHomepageItemDTO { Published = true });
        }

        // POST: HomepageManager/HomepageItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Create(int? blockId, CreateHomepageItemDTO homepageItemForCreate)
        {
            var block = _context.HomepageBlocks.FirstOrDefault(b => b.Id == blockId);
            if (block == null)
            {
                return RedirectToAction("Index", "HomepageBlocks");
            }
            ViewData["BlockTitle"] = block.Title;
            ViewData["Block"] = block;
            if (!String.IsNullOrEmpty(homepageItemForCreate.Title) && homepageItemForCreate.Title.Length != homepageItemForCreate.Title.FullTrim().Length)
            {
                ModelState.AddModelError("Title", "Tiêu đề chưa đúng định dạng");
            }
            if (ModelState.IsValid)
            {
                HomepageItem homepageItem = _mapper.Map<HomepageItem>(homepageItemForCreate);
                var maxItem = _context.HomepageItems.Where(i => i.BlockId == blockId).OrderBy(i => i.Order).LastOrDefault();
                homepageItem.Order = maxItem != null ? maxItem.Order + 1 : 1;
                homepageItem.CreatedAt = DateTime.Now;
                homepageItem.UpdatedAt = DateTime.Now;
                homepageItem.BlockId = block.Id;
                _context.Add(homepageItem);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));

            }
            ViewBag.CategorySlug = new SelectList(_context.PostCategories, "Slug", "Name", homepageItemForCreate.Slug);
            return View(homepageItemForCreate);
        }

        // GET: HomepageManager/HomepageItems/Edit/5
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Edit(int? blockId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var block = _context.HomepageBlocks.FirstOrDefault(l => l.Id == blockId);
            if (block == null)
            {
                return RedirectToAction("Index", "HomepageBlocks");
            }

            ViewData["BlockTitle"] = block.Title;
            ViewData["Block"] = block;
            var homepageItem = await _context.HomepageItems.FindAsync(id);
            if (homepageItem == null)
            {
                return NotFound();
            }
            ViewData["LinkTitle"] = homepageItem.Title;
            var homepageForEdit = _mapper.Map<UpdateHomepageItemDTO>(homepageItem);
            ViewBag.CategorySlug = new SelectList(_context.PostCategories, "Slug", "Name", homepageForEdit.Slug);
            return View(homepageForEdit);
        }

        // POST: HomepageManager/HomepageItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? blockId, int id, UpdateHomepageItemDTO homepageForEdit)
        {
            if (id != homepageForEdit.Id)
            {
                return NotFound();
            }
            var block = _context.HomepageBlocks.FirstOrDefault(l => l.Id == blockId);
            if (block == null)
            {
                return RedirectToAction("Index", "HomepageBlocks");
            }

            ViewData["BlockTitle"] = block.Title;
            ViewData["Block"] = block;
            HomepageItem homepageItem = _context.HomepageItems.FirstOrDefault(i => i.Id == id);
            ViewData["LinkTitle"] = homepageItem.Title;
            if (!String.IsNullOrEmpty(homepageForEdit.Title) && homepageForEdit.Title.Length != homepageForEdit.Title.FullTrim().Length)
            {
                ModelState.AddModelError("Title", "Tiêu đề chưa đúng định dạng");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    HomepageItem homepageItemToUpdate = _mapper.Map<UpdateHomepageItemDTO, HomepageItem>(homepageForEdit, homepageItem);
                    homepageItemToUpdate.UpdatedAt = DateTime.Now;
                    _context.Update(homepageItemToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                    {
                        Type = "success",
                        Message = "Cập nhật dữ liệu thành công"
                    });
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomepageItemExists(homepageForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.CategorySlug = new SelectList(_context.PostCategories, "Slug", "Name", homepageForEdit.Slug);
            return View(homepageForEdit);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteAsync(int? id)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần xóa, vui lòng thử lại"
                });
            }
            var homepageItem = await _context.HomepageItems.FindAsync(id);
            _context.HomepageItems.Remove(homepageItem);
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeOrder(int? blockId, int? id, int? order)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
                });
            }
            if (order <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Vui lòng nhập giá trị lớn hơn 0"
                });
            }
            if (order == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy thứ tự cần cập nhật, vui lòng thử lại"
                });
            }

            var homepageItemNewOrder = _context.HomepageItems.FirstOrDefault(i => i.Id == id);
            var homepageItemOldOrder = _context.HomepageItems.FirstOrDefault(i => i.Order == order);
            if (homepageItemNewOrder == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
                });
            }
            if (homepageItemOldOrder != null)
            {
                homepageItemOldOrder.Order = homepageItemNewOrder.Order;
                await _context.SaveChangesAsync();
            }

            homepageItemNewOrder.Order = (int)order;
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Reload = true,
                Message = "Cập nhật vị trí thành công"
            });
        }

        private bool HomepageItemExists(int id)
        {
            return _context.HomepageItems.Any(e => e.Id == id);
        }
    }
}
