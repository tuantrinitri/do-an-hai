using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.PostManager.Models;
using CMS.Data;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using CMS.Areas.Identity.Models;
using Slugify;
using Newtonsoft.Json;
using CMS.Models;
using CMS.Areas.PostManager.DTOs;
using CMS.Helpers;
using System.Security.Claims;

namespace CMS.Areas.PostManager.Components
{
    [Area("PostManager")]
    [Route("AdminCP/Posts/{action=index}/{id?}")]
    [Authorize]
    public class PartialMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public PartialMenuViewComponent(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            var unit = claims.Where(cl => cl.Type == "Unit").FirstOrDefault().Value;
            IQueryable<Post> posts = null;
            // Thu trưởng đơn vị được xem các bài viết của đơn vị
            if (User.IsInRole(RoleTypes.TT))
            {
                posts = _context.Posts
                .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
                .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
                .Where(p => p.IsDeleted != true)
                .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
                .Where(p => p.CreatedBy.Unit.ShortName == unit)
                .OrderByDescending(u => u.CreatedAt).AsNoTracking();
            }
            // Cộng tác viên chỉ xem bài viết của mình
            else if (User.IsInRole(RoleTypes.CTV))
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))               
               .Where(p => p.CreatedBy.UserName == User.Identity.Name)
               .OrderByDescending(u => u.CreatedAt).AsNoTracking();
            }
            // Admin and ban biên tập được xem tất
            else
            {
                posts = _context.Posts
               .Include(d => d.JoinPostCategories).ThenInclude(jpc => jpc.PostCategory)
               .Include(u => u.CreatedBy).ThenInclude(u => u.Unit)
               .Where(p => p.IsDeleted != true)
               .Where(p => !(p.ApprovalStatus == ApprovalStatuses.DRAFT && p.CreatedBy.UserName != User.Identity.Name))
               .OrderByDescending(u => u.CreatedAt).AsNoTracking();
            }
            //int total = posts.Count();
            List<PartialMenuDTO> menuList = new List<PartialMenuDTO>();
            int refusedCount = 0;
            int pendingCount = 0;
            // Menu của cộng tác viên
            if (User.IsInRole(RoleTypes.CTV))
            {
                refusedCount = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.REFUSED).Count();

                pendingCount = posts.Where(p => p.ApprovalStatus.Contains(ApprovalStatuses.PENDING)).Count();
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PENDING,
                    MiTitle = "Bài viết đã trình duyệt",
                    MiTotal = pendingCount,
                    MiColor = "success"
                });
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.REFUSED,
                    MiTitle = "Bài viết bị từ chối",
                    MiTotal = refusedCount,
                    MiColor = "danger"
                });
            }
            // Menu của Thủ trưởng
            else if (User.IsInRole(RoleTypes.TT))
            {
                refusedCount = posts
                .Where(p => p.ApprovalStatus == ApprovalStatuses.REFUSED && p.CreatedBy.UserName == User.Identity.Name)
                .Count();

                pendingCount = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PENDING_CTV).Count();
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PENDING,
                    MiTitle = "Bài viết chờ duyệt",
                    MiTotal = pendingCount,
                    MiColor = "warning"
                });
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.REFUSED,
                    MiTitle = "Bài viết bị từ chối",
                    MiTotal = refusedCount,
                    MiColor = "danger"
                });
            }
            // Menu của biên tập viên
            else if (User.IsInRole(RoleTypes.BTV))
            {
                pendingCount = posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PENDING_TT).Count();

                refusedCount = posts.Where(p => (p.CreatedBy.UserName == User.Identity.Name && p.ApprovalStatus == ApprovalStatuses.REFUSED)).Count();
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PENDING,
                    MiTitle = "Bài viết chờ duyệt",
                    MiTotal = pendingCount,
                    MiColor = "warning"
                });
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.REFUSED,
                    MiTitle = "Bài viết bị từ chối",
                    MiTotal = refusedCount,
                    MiColor = "danger"
                });
            }
            // Menu của tổng biên tập
            else if (User.IsInRole(RoleTypes.TBT))
            {
                pendingCount = posts.Where(p => (p.ApprovalStatus == ApprovalStatuses.PENDING_TT || p.ApprovalStatus == ApprovalStatuses.PENDING_BTV)).Count();
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PENDING,
                    MiTitle = "Bài viết chờ duyệt",
                    MiTotal = pendingCount,
                    MiColor = "warning"
                });
            }
            // Menu của admin
            else
            {
                pendingCount = await posts.Where(p => p.ApprovalStatus.Contains(ApprovalStatuses.PENDING)).CountAsync();
                refusedCount = await posts.Where(p => p.CreatedBy.UserName == User.Identity.Name && p.ApprovalStatus == ApprovalStatuses.REFUSED).CountAsync();
                int publishedCount = await posts.Where(p => p.ApprovalStatus == ApprovalStatuses.PUBLISHED).CountAsync();
                int draftCount = await posts.Where(p => p.ApprovalStatus == ApprovalStatuses.DRAFT).CountAsync();
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PUBLISHED,
                    MiTitle = "Bài viết đã xuất bản",
                    MiTotal = publishedCount,
                    MiColor = "success"
                });

                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.PENDING,
                    MiTitle = "Bài viết chờ duyệt",
                    MiTotal = pendingCount,
                    MiColor = "warning"
                });
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.REFUSED,
                    MiTitle = "Bài viết bị từ chối",
                    MiTotal = refusedCount,
                    MiColor = "danger"
                });
                menuList.Add(new PartialMenuDTO()
                {
                    MiController = "Posts",
                    MiAction = "Index",
                    MiRoute = ApprovalStatuses.DRAFT,
                    MiTitle = "Bài viết nháp",
                    MiTotal = draftCount,
                    MiColor = "secondary"
                });
            }

            return await Task.FromResult((IViewComponentResult)View("Default", menuList));
        }
    }
}
