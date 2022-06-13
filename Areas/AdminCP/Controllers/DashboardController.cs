using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMS.Areas.AdminCP.DTOs;
using CMS.Areas.ContactManager.Models;
 
using CMS.Areas.Identity.Models;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using CMS.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;

namespace CMS.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    [Route("/AdminCP/{action=index}")]
    [DefaultBreadcrumb()]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            IQueryable<Post> posts = null;
            IQueryable<Post> model = null;
            int documents = 0;
            int users = 0;
            int contacts = 0;
            int questions = 0;

      
            // Cộng tác viên chỉ xem bài viết của mình
             if (User.IsInRole(RoleTypes.CTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy)
               .Where(p => p.IsDeleted != true)
               .Where(p => p.CreatedBy.UserName == User.Identity.Name)
               .OrderByDescending(u => u.ApprovalAt)
               .AsNoTracking();
                    ViewData["ActionTitle"] = "Chức năng";
                    ViewData["TableTitle"] = "Danh sách bài viết bị từ chối";
                contacts = _context.Contacts.Count();
                ViewData["DashBoard"] = new List<DashboardItemDTO>()
                {
                    // card bai viet mới
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Create",
                       Route = "",
                       IconClass = "icon-plus3",
                       Title = "Bài viết mới",
                       Total = "TẠO",
                       BgColorClass = "bg-info-400"
                   },
                    // tất cả bài viết
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = "",
                       IconClass = "icon-file-text2",
                       Title = "Tất cả bài viết",
                       Total = posts.Count().ToString(),
                       BgColorClass = "bg-indigo-300"
                   },
                   new DashboardItemDTO()
                   {
                       Area = "ContactManager",
                       Controller = "Contacts",
                       Action = "Index",
                       Route = "",
                       IconClass = "icon-envelop2",
                       Title = "Liên hệ",
                       Total = contacts.ToString(),
                       BgColorClass = "bg-blue-400"
                   }
                  
                };
   
            }
             // menu của admin
            else
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy)
               .Where(p => p.IsDeleted != true)
               .Where(p => p.CreatedBy.UserName != User.Identity.Name)
               //   .OrderByDescending(u => u.Id)
               .AsNoTracking();
                contacts = _context.Contacts.Count();
                users = _context.Users.Count();
               var counterPosts = _context.Posts.Count();
                ViewData["ActionTitle"] = "Thống kê";
                ViewData["DashBoard"] = new List<DashboardItemDTO>()
                {
                    // card bai viet
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = "",
                       IconClass = "icon-file-text2",
                       Title = "Bài viết",
                       Total = counterPosts.ToString(),
                       BgColorClass = "bg-indigo-300"
                   },
                 
                   new DashboardItemDTO()
                   {
                       Area = "Identity",
                       Controller = "Users",
                       Action = "Index",
                       Route = "",
                       IconClass = "icon-user",
                       Title = "Tài khoản",
                       Total = users.ToString(),
                       BgColorClass = "bg-success-400"
                   }                   ,
                    // card lien he
                   new DashboardItemDTO()
                   {
                       Area = "ContactManager",
                       Controller = "Contacts",
                       Action = "Index",
                       Route = "",
                       IconClass = "icon-envelop2",
                       Title = "Liên hệ",
                       Total = contacts.ToString(),
                       BgColorClass = "bg-blue-400"
                   }
                };
            }
            return View(model);
        }
    }
}
