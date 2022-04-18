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

namespace CMS.Areas.HomepageManager.Controllers
{
    [Area("HomepageManager")]
    [Route("AdminCP/HomepageBlocks/{action=index}/{id?}")]
    [Authorize(Roles = "admin")]
    public class HomepageBlocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomepageBlocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HomapageManager/HomepageBlocks
        [Breadcrumb("Trang chủ/Khối")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.HomepageBlocks.OrderBy(b => b.Id).ToListAsync());
        }

        // GET: HomepageManager/HomepageBlocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homepageBlock = await _context.HomepageBlocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homepageBlock == null)
            {
                return NotFound();
            }

            return View(homepageBlock);
        }

        // GET: HomepageManager/HomepageBlocks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomepageManager/HomepageBlocks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Alias,Title,Type,Description,Published")] HomepageBlock homepageBlock)
        {
            if (ModelState.IsValid)
            {
                _context.Add(homepageBlock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homepageBlock);
        }

        // GET: HomepageManager/HomepageBlocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homepageBlock = await _context.HomepageBlocks.FindAsync(id);
            if (homepageBlock == null)
            {
                return NotFound();
            }
            return View(homepageBlock);
        }

        // POST: HomepageManager/HomepageBlocks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Alias,Title,Type,Description,Published")] HomepageBlock homepageBlock)
        {
            if (id != homepageBlock.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(homepageBlock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomepageBlockExists(homepageBlock.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(homepageBlock);
        }

        // GET: HomepageManager/HomepageBlocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homepageBlock = await _context.HomepageBlocks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homepageBlock == null)
            {
                return NotFound();
            }

            return View(homepageBlock);
        }

        // POST: HomepageManager/HomepageBlocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var homepageBlock = await _context.HomepageBlocks.FindAsync(id);
            _context.HomepageBlocks.Remove(homepageBlock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomepageBlockExists(int id)
        {
            return _context.HomepageBlocks.Any(e => e.Id == id);
        }
    }
}
