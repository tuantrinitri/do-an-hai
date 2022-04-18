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
using CMS.Areas.WidgetManager.DTOs;
using CMS.Areas.WidgetManager.Models;

namespace CMS.Areas.PostManager.Controllers
{
    [Area("WidgetManager")]
    [Route("AdminCP/RightWidget/{action=index}/{id?}")]
    [Authorize]
    public class RightWidgetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SlugHelper _slugHelper = new SlugHelper();

        public RightWidgetController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //[Route("/")]
        // GET: WidgetManager/RightWidget
        [Breadcrumb("Widget bên phải")]
        public async Task<IActionResult> Index()
        {
            var links = _context.RightWidgets.OrderBy(l => l.Priority).ToList();
            int i = 1;
            foreach (var iLink in links)
            {
                iLink.Priority = i;
                await _context.SaveChangesAsync();
                i++;
            }

            return View(_context.RightWidgets.OrderBy(l => l.Priority).ToList());
        }

        // GET: WidgetManager/RightWidget/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rightWidget = await _context.RightWidgets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rightWidget == null)
            {
                return NotFound();
            }

            return View(rightWidget);
        }

        // GET: WidgetManager/RightWidget/Create
        [Breadcrumb("Tạo mới")]
        public IActionResult Create()
        {
            ViewData["CategorySlug"] = new SelectList(_context.PostCategories, "Slug", "Name");
            return View(new CreateRightWidgetDTO { Activated = true });
        }

        // POST: WidgetManager/RightWidget/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRightWidgetDTO widgetDTO)
        {
            if (ModelState.IsValid)
            {
                RightWidget widget = _mapper.Map<RightWidget>(widgetDTO);            
                _context.Add(widget);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorySlug"] = new SelectList(_context.PostCategories, "Slug", "Name", widgetDTO.CategorySlug);
            return View(widgetDTO);
        }

        // GET: WidgetManager/RightWidget/Edit/5
        [Breadcrumb("Sửa widget")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var widget = await _context.RightWidgets.FindAsync(id);
            if (widget == null)
            {
                return NotFound();
            }
            UpdateRightWidgetDTO widgetDTO = _mapper.Map<UpdateRightWidgetDTO>(widget);
            ViewData["CategorySlug"] = new SelectList(_context.PostCategories, "Slug", "Name", widgetDTO.CategorySlug);
            return View(widgetDTO);
        }

        // POST: WidgetManager/RightWidget/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRightWidgetDTO widgetDTO)
        {
            if (id != widgetDTO.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    RightWidget widget = _context.RightWidgets.FirstOrDefault(pc => pc.Id == id);
                    RightWidget widgetToUpdate = _mapper.Map<UpdateRightWidgetDTO, RightWidget>(widgetDTO, widget);                   
                    _context.Update(widgetToUpdate);
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
                    if (!RightWidgetExists(widgetDTO.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["CategorySlug"] = new SelectList(_context.PostCategories, "Slug", "Name", widgetDTO.CategorySlug);
            return View(widgetDTO);
        }

        // POST: WidgetManager/RightWidget/Delete/5
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

            var widgetToDelete = await _context.RightWidgets.FindAsync(id);
            _context.RightWidgets.Remove(widgetToDelete);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeOrder(int? id, int? order)
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

            var newOrder = _context.RightWidgets.FirstOrDefault(i => i.Id == id);
            var oldOrder = _context.RightWidgets.FirstOrDefault(i => i.Priority == order);
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
                oldOrder.Priority = newOrder.Priority;
                await _context.SaveChangesAsync();
            }

            newOrder.Priority = (int)order;
            await _context.SaveChangesAsync();

            return Json(new
            {
                Status = true,
                Reload = true,
                Message = "Cập nhật vị trí thành công"
            });
        }

        private bool RightWidgetExists(int id)
        {
            return _context.RightWidgets.Any(e => e.Id == id);
        }
    }
}
