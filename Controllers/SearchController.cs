using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using CMS.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("tim-kiem")]
        public IActionResult Index(string q, int page = 1)
        {
            var results = _context.Posts
            .Where(p => p.Title.Contains(q))
            .Where(pc => pc.IsDeleted != true)
            .ToList().AsQueryable();
            ViewBag.q = q;
            ViewBag.results = results;
            ViewData["Title"] = "\"" + q + "\" | Tìm kiếm";
            var pagination = PaginatedList<Post>.Create(results, page, 10);
            return View(pagination);
        }
    }
}
