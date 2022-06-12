using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CMS.Models;
using SmartBreadcrumbs.Attributes;
using CMS.Data;
using CMS.Areas.SettingManager.Models;
using CMS.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // khởi chạy lần đầu cho toàn bộ các thông tin website
            var globalSetting = await _context.GlobalSettings.OrderByDescending(z =>z.Id).FirstOrDefaultAsync();
            if (globalSetting == null)
            {
                globalSetting = new GlobalSetting
                {
                    LogoFooter = "",
                    LogoHeader = "",
                    SiteType = "Trang tin điện tử",
                    UnitName = "Du Lịch Quảng Ninh",
                    UnitNameEn = "Quảng Ninh Travel",
                    Email = "thuhai@qn.vn",
                    LastModifiedAt = DateTime.Now
                };
                _context.GlobalSettings.Add(globalSetting);
                _context.SaveChanges();
            }
            // lấy danh sách bài viết
            var posts =  _context.Posts.OrderByDescending(x => x.CreatedAt).AsNoTracking();
            
            // lấy danh sách category
            ViewBag.NewPost =  posts.Take(3).ToList();
            ViewBag.PostFeatured =  posts.Where(x => x.IsFeatured == true).ToList();
            return await Task.FromResult((IActionResult) View("Index"));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}