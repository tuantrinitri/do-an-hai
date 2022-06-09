using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CMS.Models;
using SmartBreadcrumbs.Attributes;
using CMS.Data;
using CMS.Areas.SettingManager.Models;
using CMS.Helpers;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.StaticPages.Models;
using CMS.Areas.PostManager.Models;

namespace CMS.Controllers
{
    [Route("chuyen-muc")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ApplicationDbContext _context;

        public CategoryController(ILogger<CategoryController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{*slug}")]
        public IActionResult Index(string slug, int page = 1)
        {
            var posts = _context.Posts
                .Include(post => post.JoinPostCategories)
                .ThenInclude(jp => jp.PostCategory)
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.CreatedAt);
            var category = new PostCategory()
            {
                Name = "Tất cả bài viết",
                Slug = ""
            };

            if (slug != null)
            {
                category = _context.PostCategories.Where(c => c.Slug == slug).FirstOrDefault();
                posts = _context.Posts
                    .Include(post => post.JoinPostCategories)
                    .ThenInclude(jp => jp.PostCategory)
                    .Where(p => p.IsDeleted != true)
                    .Where(p => p.JoinPostCategories.Any(j => j.PostCategory.Slug == slug))
                    .OrderByDescending(p => p.CreatedAt);
            }

            if (posts == null)
            {
                return NotFound();
            }

            var pagination = PaginatedList<Post>.Create(posts, page, 10);
            ViewData["Category"] = category;
            return View(pagination);
        }

/*         [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        } */
    }
}