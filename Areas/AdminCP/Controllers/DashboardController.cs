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
            var unit = claims.Where(cl => cl.Type == "Unit").FirstOrDefault().Value;
            IQueryable<Post> posts = null;
            IQueryable<Post> model = null;
            int documents = 0;
            int users = 0;
            int contacts = 0;
            int questions = 0;

            // Thu trưởng đơn vị được xem các bài viết của đơn vị
            if (User.IsInRole(RoleTypes.TT))
            {
                posts = _context.Posts
                .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
                .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
                .Where(p => p.IsDeleted != true)
                .Where(p => p.CreatedBy.Unit.ShortName == unit)
                .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking();
                ViewData["ActionTitle"] = "Chức năng";
                ViewData["TableTitle"] = "Danh sách bài viết chờ duyệt";
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
                       Total = "Tạo",
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
                    // bài viết đã trình duyệt
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PENDING,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã trình duyệt",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PENDING_TT || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   }                   ,
                    // bài viết bị từ chối
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.REFUSED,
                       IconClass = "icon-blocked",
                       Title = "Bài bị từ chối",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName == User.Identity.Name)
                       .Count().ToString(),
                       BgColorClass = "bg-danger-600"
                   },
                    // bài viết đã xuất bản
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PUBLISHED,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PUBLISHED ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   },
                    // bài viết nháp
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.DRAFT,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName == User.Identity.Name ).Count().ToString(),
                       BgColorClass = "bg-blue-400"
                   }
                };
                model = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PENDING_CTV);
            }

            // Cộng tác viên chỉ xem bài viết của mình
            else if (User.IsInRole(RoleTypes.CTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => p.CreatedBy.UserName == User.Identity.Name)
               .OrderByDescending(u => u.ApprovalAt)
               .AsNoTracking();
                ViewData["ActionTitle"] = "Chức năng";
                ViewData["TableTitle"] = "Danh sách bài viết bị từ chối";
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
                    // Bài viết đã trình duyệt
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PENDING,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã trình duyệt",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PENDING_BTV || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV || p.ApprovalStatus == ApprovalStatuses.PENDING_TT ).Count().ToString(),
                       BgColorClass = "bg-success-400"
                   },
                    // bài viết bị từ chối
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.REFUSED,
                       IconClass = "icon-blocked",
                       Title = "Bài viết bị từ chối",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.REFUSED ).Count().ToString(),
                       BgColorClass = "bg-danger-600"
                   },
                    // bài viết đã xuất bản
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PUBLISHED,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PUBLISHED ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   }
                };
                model = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.REFUSED);
            }
            // BTV 
            else if (User.IsInRole(RoleTypes.BTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.CreatedAt)
               .AsNoTracking();
                ViewData["ActionTitle"] = "Chức năng";
                ViewData["TableTitle"] = "Danh sách bài viết chờ duyệt";
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
                    // bài viết đã trình duyệt
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PENDING,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã trình duyệt",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PENDING_TT || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   }                   ,
                    // bài viết bị từ chối
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.REFUSED,
                       IconClass = "icon-blocked",
                       Title = "Bài bị từ chối",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.REFUSED ).Count().ToString(),
                       BgColorClass = "bg-danger-600"
                   },
                    // bài viết đã xuất bản
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PUBLISHED,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PUBLISHED ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   },
                    // bài viết nháp
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.DRAFT,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName == User.Identity.Name ).Count().ToString(),
                       BgColorClass = "bg-blue-400"
                   }
                };
                model = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PENDING_TT);
            }
            // menu của tổng biên tập
            else if (User.IsInRole(RoleTypes.TBT))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.CreatedAt)
               .AsNoTracking();
                ViewData["ActionTitle"] = "Chức năng";
                ViewData["TableTitle"] = "Danh sách bài viết chờ duyệt";
                ViewData["DashBoard"] = new List<DashboardItemDTO>()
                {
                    // bài viết mới
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
                    // bài viết đã xuất bản
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PUBLISHED,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết đã xuất bản",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PUBLISHED ).Count().ToString(),
                       BgColorClass = "bg-teal-400"
                   }                   ,
                    // bài viết nháp
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.DRAFT,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết nháp",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.DRAFT ).Count().ToString(),
                       BgColorClass = "bg-blue-400"
                   },
                    // bài viết chờ duyệt
                   new DashboardItemDTO()
                   {
                       Area = "PostManager",
                       Controller = "Posts",
                       Action = "Index",
                       Route = ApprovalStatuses.PENDING,
                       IconClass = "icon-file-text2",
                       Title = "Bài viết chờ duyệt",
                       Total = posts.Where( p=>p.ApprovalStatus == ApprovalStatuses.PENDING_TT || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV ).Count().ToString(),
                       BgColorClass = "bg-orange-400"
                   }
                };
                model = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PENDING_TT || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV);
            }
            // menu của admin
            else
            {
                posts = _context.Posts
               //  .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               //  .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName != User.Identity.Name))
               //   .OrderByDescending(u => u.Id)
               .AsNoTracking();

             
                contacts = _context.Contacts.Count();
                users = _context.Users.Count();
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
                       Total = posts.Count().ToString(),
                       BgColorClass = "bg-indigo-300"
                   },
                    // card tai lieu
                   // new DashboardItemDTO()
                   // {
                   //     Area = "DocumentManager",
                   //     Controller = "Documents",
                   //     Action = "Index",
                   //     Route = "",
                   //     IconClass = "icon-files-empty",
                   //     Title = "Tài liệu",
                   //     Total = documents.ToString(),
                   //     BgColorClass = "bg-teal-400"
                   // },
                    // card tai khoan
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
                   },
                     // card cau hoi
                   // new DashboardItemDTO()
                   // {
                   //     Area = "QAManager",
                   //     Controller = "QAAdmin",
                   //     Action = "Index",
                   //     Route = "",
                   //     IconClass = "icon-question7",
                   //     Title = "Nội dung hỏi đáp",
                   //     Total = questions.ToString(),
                   //     BgColorClass = "bg-indigo-400"
                   // },
                };
            }
            return View(model);
        }
    }
}
