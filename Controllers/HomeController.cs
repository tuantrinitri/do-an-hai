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
            var globalSetting = await _context.GlobalSettings.FirstOrDefaultAsync();
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
            var posts = await _context.Posts
                .Include(p => p.JoinPostCategories)
                .ThenInclude(p => p.PostCategory)
                .Where(pc => pc.IsDeleted != true)
                .AsNoTracking().ToListAsync();

            // lấy danh sách category
            ViewBag.NewPost = await _context.Posts.OrderByDescending(x =>x.CreatedAt).AsNoTracking().ToListAsync();
            
            // lấy danh sách bài viết nổi bật 

            ViewBag.PostFeatured = posts.Where(x => x.IsFeatured == true);
            
            
            //   var homepageItems = await _context.HomepageItems.Where(hi => hi.Published == true).Include(item => item.Block).ToListAsync();
            // // Get homepage block 1 item
            // var homepageBlock1 = new List<HomepageItemViewModel>();
            // var block1Items = homepageItems.Where(i => i.Block.Alias == "BLOCK_HOME_1").OrderBy(i => i.Order).ToList();
            // foreach (var item in block1Items)
            //     homepageBlock1.Add(new HomepageItemViewModel()
            //     {
            //         CategorySlug = item.Slug,
            //         Title = item.Title,
            //         Type = item.Type,
            //         Posts = item.Slug == null ? posts.Where(p => p.IsFeatured == true).OrderByDescending(p => p.CreatedAt).Take(item.NoPosts).ToList()
            //                 : posts.Where(p => p.JoinPostCategories.Any(j => j.PostCategory.Slug == item.Slug)).OrderBy(p => p.IsFeatured).OrderByDescending(p => p.CreatedAt).Take(item.NoPosts).ToList()
            //
            //     });
            // ViewBag.HomepageBlock1 = homepageBlock1;
            // Get homepage block 2 item 
            // ViewBag.photo = await _context.GalleryItems.Where(s => s.Gallery.Alias == GalleriesAlias.PHOTO).Where(s => s.Published == true).OrderBy(s => s.Order).ToListAsync();
            // ViewBag.video = await _context.GalleryItems.Where(s => s.Gallery.Alias == GalleriesAlias.VIDEO).Where(s => s.Published == true).OrderBy(s => s.Order).ToListAsync();
           
            
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