using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.Identity.Models;
using CMS.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using SmartBreadcrumbs.Attributes;
using CMS.Helpers;
using CMS.Areas.Identity.DTOs;
using Newtonsoft.Json;
using CMS.Models;

namespace CMS.Areas.Identity.Controllers
{
    [Area("Identity")]
    [Route("AdminCP/JobTitles/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class JobTitlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public JobTitlesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Identity/JobTitles
        [Breadcrumb("Chức vụ")]
        public async Task<IActionResult> Index(string q, int? activated, int? page)
        {
            var jobTitles = _context.JobTitles.OrderByDescending(u => u.Id);

            if (!String.IsNullOrEmpty(q))
            {
                ViewData["filter_q"] = q;
                jobTitles = (IOrderedQueryable<JobTitle>)jobTitles.Where(d => d.Title.Contains(q));
            }
            if (activated != null)
            {
                ViewData["filter_activated"] = activated;
                jobTitles = (IOrderedQueryable<JobTitle>)jobTitles.Where(d => d.Activated == (activated == 1));
            }
            ViewData["Status"] = new SelectList(new List<SelectListItem>
                {
                    new SelectListItem{ Value = "1", Text = "Hoạt động"},
                    new SelectListItem{ Value = "0", Text = "Tạm khóa"}
                }, "Value", "Text", new { Value = activated });
            ViewBag.totalitem = jobTitles.Count();
            return View(PaginatedList<JobTitle>.Create(jobTitles.AsNoTracking(), page ?? 1, 10));
        }

        // GET: Identity/JobTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobTitle = await _context.JobTitles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobTitle == null)
            {
                return NotFound();
            }

            return View(jobTitle);
        }

        // GET: Identity/Units/Create
        [Breadcrumb("ViewData.Title")]
        public IActionResult Create()
        {
            return View(new JobTitleForCreateDTO { Activated = true });
        }

        // POST: Identity/JobTitles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobTitleForCreateDTO jobTitleForCreate)
        {
            if (ModelState.IsValid)
            {
                var jobTitle = _mapper.Map<JobTitle>(jobTitleForCreate);
                _context.Add(jobTitle);
                await _context.SaveChangesAsync();
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Tạo dữ liệu thành công"
                });
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitleForCreate);
        }

        // GET: Identity/Units/Edit/5
        [Breadcrumb("ViewData.Title")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (jobTitle == null)
            {
                return NotFound();
            }
            var jobTitleForEdit = _mapper.Map<JobTitleForEditDTO>(jobTitle);
            return View(jobTitleForEdit);
        }

        // POST: Identity/Units/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Breadcrumb("ViewData.Title")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, JobTitleForEditDTO jobTitleForEdit)
        {
            if (id != jobTitleForEdit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    JobTitle jobTitle = _context.JobTitles.FirstOrDefault(u => u.Id == id);
                    JobTitle jobTitleToUpdate = _mapper.Map<JobTitleForEditDTO, JobTitle>(jobTitleForEdit, jobTitle);
                    _context.Update(jobTitleToUpdate);
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
                    if (!JobTitleExists(jobTitleForEdit.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(jobTitleForEdit);
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

            var jobTitle = await _context.JobTitles.FindAsync(id);
            if (_context.Users.Where(u => u.JobTitleId == id).Count() > 0)
            {
                return Json(new
                {
                    Status = false,
                    Message = "Chức vụ đã được sử dụng, không thể xóa"
                });
            }
            _context.JobTitles.Remove(jobTitle);
            await _context.SaveChangesAsync();
            return Json(new
            {
                Status = true,
                Message = "Đã xóa dữ liệu thành công"
            });
        }

        private bool JobTitleExists(int id)
        {
            return _context.JobTitles.Any(e => e.Id == id);
        }
    }
}
