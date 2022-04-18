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

namespace CMS.Controllers
{
    [Route("trang")]
    public class PageController : Controller
    {
        private readonly ILogger<PageController> _logger;
        private readonly ApplicationDbContext _context;

        public PageController(ILogger<PageController> logger, ApplicationDbContext context)
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
            StaticPage staticPage = _context.StaticPages
                .FirstOrDefault(m => m.Slug == slug);
            if (staticPage == null)
            {
                return NotFound();
            }
            return View(staticPage);    
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
