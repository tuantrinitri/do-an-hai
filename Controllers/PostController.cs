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
    [Route("bai-viet")]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly ApplicationDbContext _context;

        public PostController(ILogger<PostController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("{slug}")]
        public IActionResult Details(string slug)
        {
              if (slug == null)
            {
                return NotFound();
            }
            Post post = _context.Posts
                .Include( post=> post.JoinPostCategories)
                .ThenInclude( jp => jp.PostCategory)
                .Where( p => p.IsDeleted != true)
                .FirstOrDefault(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
