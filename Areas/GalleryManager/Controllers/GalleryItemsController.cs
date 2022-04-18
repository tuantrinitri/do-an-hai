using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.GalleryManager.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs.Attributes;
using CMS.Areas.GalleryManager.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Helpers;

namespace CMS.Areas.GalleryManager.Controllers
{
    [Area("GalleryManager")]
    [Route("AdminCP/Galleries/{galleryId?}/Items/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class GalleryItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GalleryItemsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: GalleryManager/GalleryItems
        [Breadcrumb("ViewData.GalleryTitle", FromController = typeof(GalleriesController), FromAction = "Index")]
        public async Task<IActionResult> Index(int? galleryId, int? page)
        {
            var gallery = _context.Galleries.FirstOrDefault(g => g.Id == galleryId);
            if (gallery == null)
            {
                return RedirectToAction("Index", "Galleries");
            }
            var listGalleryItems = _context.GalleryItems.Where(l => l.GalleryId == galleryId).OrderBy(l => l.Order).ToList();
            int i = 1;
            foreach (var iItem in listGalleryItems)
            {
                iItem.Order = i;
                _context.SaveChanges();
                i++;
            }
            ViewData["GalleryTitle"] = gallery.Title;
            ViewData["Gallery"] = gallery;
            var galleryItems = _context.GalleryItems.Include(g => g.Gallery).Where(i => i.GalleryId == galleryId).OrderBy(i => i.Order);

            ViewBag.totalitem = galleryItems.Count();
            return View(PaginatedList<GalleryItem>.Create(galleryItems.AsNoTracking(), page ?? 1, 10));
        }

        // GET: GalleryManager/GalleryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galleryItem = await _context.GalleryItems
                .Include(g => g.Gallery)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galleryItem == null)
            {
                return NotFound();
            }

            return View(galleryItem);
        }

        // GET: GalleryManager/GalleryItems/Create
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public IActionResult Create(int? galleryId)
        {
            var gallery = _context.Galleries.FirstOrDefault(g => g.Id == galleryId);
            if (gallery == null)
            {
                return RedirectToAction("Index", "Galleries");
            }

            ViewData["GalleryTitle"] = gallery.Title;
            ViewData["Gallery"] = gallery;
            return View(new GalleryItemForCreateDTO { Published = true });
        }

        // POST: GalleryManager/GalleryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Create(int? galleryId, GalleryItemForCreateDTO galleryItemForCreate)
        {
            var gallery = _context.Galleries.FirstOrDefault(g => g.Id == galleryId);
            if (gallery == null)
            {
                return RedirectToAction("Index", "Galleries");
            }
            ViewData["GalleryTitle"] = gallery.Title;
            ViewData["Gallery"] = gallery;
            if (gallery.Type == "photo" && String.IsNullOrEmpty(galleryItemForCreate.Image))
            {
                ModelState.AddModelError("Image", "Chưa chọn hình ảnh");
            }
            if (!String.IsNullOrEmpty(galleryItemForCreate.Image) && !galleryItemForCreate.Image.CheckTypeFile(new List<string> { "jpg", "png" }))
            {
                ModelState.AddModelError("Image", "Định dạng không hợp lệ");
            }
            if (gallery.Type == "video" && String.IsNullOrEmpty(galleryItemForCreate.Video))
            {
                ModelState.AddModelError("Video", "Chưa chọn video");
            }
            if (gallery.Type == "video" && !galleryItemForCreate.Video.CheckTypeFile(new List<string> { "mp4" }))
            {
                ModelState.AddModelError("Video", "Định dạng không hợp lệ");
            }
            if (ModelState.IsValid)
            {
                GalleryItem galleryItem = _mapper.Map<GalleryItem>(galleryItemForCreate);
                var maxItem = _context.GalleryItems.Where(i => i.GalleryId == galleryId).OrderBy(i => i.Order).LastOrDefault();
                galleryItem.Order = maxItem != null ? maxItem.Order + 1 : 1;
                galleryItem.CreatedAt = DateTime.Now;
                galleryItem.LastModifiedAt = DateTime.Now;
                galleryItem.GalleryId = gallery.Id;
                _context.Add(galleryItem);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            return View(galleryItemForCreate);
        }

        // GET: GalleryManager/GalleryItems/Edit/5
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        public async Task<IActionResult> Edit(int? galleryId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var gallery = _context.Galleries.FirstOrDefault(g => g.Id == galleryId);
            if (gallery == null)
            {
                return RedirectToAction("Index", "Galleries");
            }
            ViewData["GalleryTitle"] = gallery.Title;
            ViewData["Gallery"] = gallery;
            var galleryItem = await _context.GalleryItems.FindAsync(id);
            if (galleryItem == null)
            {
                return NotFound();
            }
            ViewData["GalleryItemTitle"] = galleryItem.Title;
            var galleryItemForEdit = _mapper.Map<GalleryItemForEditDTO>(galleryItem);
            return View(galleryItemForEdit);
        }

        // POST: GalleryManager/GalleryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Breadcrumb("ViewData.Title", FromAction = "Index")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? galleryId, int id, GalleryItemForEditDTO galleryItemForEdit)
        {
            if (id != galleryItemForEdit.Id)
            {
                return NotFound();
            }
            var gallery = _context.Galleries.FirstOrDefault(g => g.Id == galleryId);
            if (gallery == null)
            {
                return RedirectToAction("Index", "Galleries");
            }
            ViewData["GalleryTitle"] = gallery.Title;
            ViewData["Gallery"] = gallery;

            if (gallery.Type == "photo" && String.IsNullOrEmpty(galleryItemForEdit.Image))
            {
                ModelState.AddModelError("Image", "Chưa chọn hình ảnh");
            }
            if (!String.IsNullOrEmpty(galleryItemForEdit.Image) && !galleryItemForEdit.Image.CheckTypeFile(new List<string> { "jpg", "png" }))
            {
                ModelState.AddModelError("Image", "Định dạng không hợp lệ");
            }
            if (gallery.Type == "video" && String.IsNullOrEmpty(galleryItemForEdit.Video))
            {
                ModelState.AddModelError("Video", "Chưa chọn video");
            }
            if (gallery.Type == "video" && !galleryItemForEdit.Video.CheckTypeFile(new List<string> { "mp4" }))
            {
                ModelState.AddModelError("Video", "Định dạng không hợp lệ");
            }
            GalleryItem galleryItem = _context.GalleryItems.FirstOrDefault(i => i.Id == id);
            ViewData["GalleryItemTitle"] = galleryItem.Title;
            if (ModelState.IsValid)
            {
                try
                {
                    GalleryItem galleryItemToUpdate = _mapper.Map<GalleryItemForEditDTO, GalleryItem>(galleryItemForEdit, galleryItem);
                    galleryItemToUpdate.LastModifiedAt = DateTime.Now;
                    _context.Update(galleryItemToUpdate);
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
                    if (!GalleryItemExists(galleryItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(galleryItemForEdit);
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
            var galleryItem = await _context.GalleryItems.FindAsync(id);
            _context.GalleryItems.Remove(galleryItem);
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeOrder(int? galleryId, int? id, int? order)
        {
            if (id == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
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
            if (order <= 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Vui lòng nhập giá trị lớn hơn 0"
                });
            }
            var newOrder = _context.GalleryItems.Where(i => i.GalleryId == galleryId).FirstOrDefault(i => i.Id == id);
            var oldOrder = _context.GalleryItems.Where(i => i.GalleryId == galleryId).FirstOrDefault(i => i.Order == order);
            if (newOrder == null)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Không tìm thấy bản ghi cần update, vui lòng thử lại"
                });
            }
            if (oldOrder != null)
            {
                oldOrder.Order = newOrder.Order;
                await _context.SaveChangesAsync();
            }

            newOrder.Order = (int)order;
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Reload = true,
                Message = "Cập nhật thứ tự thành công"
            });
        }

        private bool GalleryItemExists(int id)
        {
            return _context.GalleryItems.Any(e => e.Id == id);
        }
    }
}
