using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.SettingManager.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using CMS.Models;

namespace CMS.Areas.SettingManager.Controllers
{
    [Area("SettingManager")]
    [Route("AdminCP/Settings/{action=index}")]
    [Authorize(Roles = "admin")]
    public class GlobalSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GlobalSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SettingManager/GlobalSettings
        [Breadcrumb("Cấu hình thông tin website")]
        public async Task<IActionResult> Index()
        {
            var globalSetting = await _context.GlobalSettings.FirstOrDefaultAsync();
            if (globalSetting == null)
            {
                globalSetting = new GlobalSetting
                {
                    LogoFooter = "",
                    LogoHeader = "",
                    SiteType = "",
                    UnitName = "",
                    UnitNameEn = "",
                    LastModifiedAt = DateTime.Now
                };
                _context.GlobalSettings.Add(globalSetting);
                _context.SaveChanges();
            }
            return View(globalSetting);
        }

        // GET: SettingManager/GlobalSettings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalSetting = await _context.GlobalSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (globalSetting == null)
            {
                return NotFound();
            }

            return View(globalSetting);
        }

        // GET: SettingManager/GlobalSettings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SettingManager/GlobalSettings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,URL,UnitName,UnitNameEn,LogoHeader,LogoFooter,SiteType,Email,Address,PhoneNumber,Fax,MoreInfo,LastModifiedAt")] GlobalSetting globalSetting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(globalSetting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(globalSetting);
        }

        // GET: SettingManager/GlobalSettings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalSetting = await _context.GlobalSettings.FindAsync(id);
            if (globalSetting == null)
            {
                return NotFound();
            }
            return View(globalSetting);
        }

        // POST: SettingManager/GlobalSettings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Breadcrumb("Cấu hình thông tin website")]
        public async Task<IActionResult> Index(int id, GlobalSetting globalSetting)
        {
            if (id != globalSetting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    globalSetting.LastModifiedAt = DateTime.Now;
                    _context.Update(globalSetting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GlobalSettingExists(globalSetting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["FlashData"] = JsonConvert.SerializeObject(new FlashData
                {
                    Type = "success",
                    Message = "Cập nhật thông tin website thành công"
                });
                return RedirectPermanent("/AdminCP");
            }
            return View(globalSetting);
        }

        // GET: SettingManager/GlobalSettings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var globalSetting = await _context.GlobalSettings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (globalSetting == null)
            {
                return NotFound();
            }

            return View(globalSetting);
        }

        // POST: SettingManager/GlobalSettings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var globalSetting = await _context.GlobalSettings.FindAsync(id);
            _context.GlobalSettings.Remove(globalSetting);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GlobalSettingExists(int id)
        {
            return _context.GlobalSettings.Any(e => e.Id == id);
        }
    }
}
