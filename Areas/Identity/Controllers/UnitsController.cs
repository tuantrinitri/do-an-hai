using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.Identity.Models;
using CMS.Data;
using CMS.Helpers;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using CMS.Areas.Identity.DTOs;
using AutoMapper;
using Newtonsoft.Json;
using CMS.Models;

namespace CMS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("AdminCP/Units/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class UnitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UnitsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Identity/Units
        [Breadcrumb("Đơn vị")]
        public async Task<IActionResult> Index(string q, int? activated, int? page)
        {
            var units = _context.Units.OrderByDescending(u => u.Id);

            if (!String.IsNullOrEmpty(q))
            {
                ViewData["filter_q"] = q;
                units = (IOrderedQueryable<Unit>)units.Where(d => d.Title.Contains(q));
            }
            if (activated != null)
            {
                ViewData["filter_activated"] = activated;
                units = (IOrderedQueryable<Unit>)units.Where(d => d.Activated == (activated == 1));
            }
            ViewData["Status"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = "1", Text = "Hoạt động"},
                    new SelectListItem{ Value = "0", Text = "Tạm khóa"}
                }, "Value", "Text", new { Value = activated });
            ViewBag.totalitem = units.Count();
            return View(PaginatedList<Unit>.Create(units.AsNoTracking(), page ?? 1, 10));
        }

        // GET: Identity/Units/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unit == null)
            {
                return NotFound();
            }

            return View(unit);
        }

        // GET: Identity/Units/Create
        [Breadcrumb("ViewData.Title")]
        public IActionResult Create()
        {
            return View(new UnitForCreateDTO { Activated = true });
        }

        // POST: Identity/Units/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitForCreateDTO unitForCreate)
        {
            if (ModelState.IsValid)
            {
                var unit = _mapper.Map<Unit>(unitForCreate);
                _context.Add(unit);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            return View(unitForCreate);
        }

        // GET: Identity/Units/Edit/5
        [Breadcrumb("ViewData.Title")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            var unitForEdit = _mapper.Map<UnitForEditDTO>(unit);
            return View(unitForEdit);
        }

        // POST: Identity/Units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UnitForEditDTO unitForEdit)
        {
            if (id != unitForEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Unit unit = _context.Units.FirstOrDefault(u => u.Id == id);
                    Unit unitToUpdate = _mapper.Map<UnitForEditDTO, Unit>(unitForEdit, unit);
                    _context.Update(unitToUpdate);
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
                    if (!UnitExists(unitForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(unitForEdit);
        }

        [HttpPost]
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

            var unit = await _context.Units.FindAsync(id);
            if (_context.Users.Where(u => u.UnitId == id).Count() > 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Đơn vị đã được sử dụng, không thể xóa"
                });
            }
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        private bool UnitExists(int id)
        {
            return _context.Units.Any(e => e.Id == id);
        }
    }
}
