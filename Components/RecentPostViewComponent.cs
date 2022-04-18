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

namespace CMS.Components
{
    public class RecentPostViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public RecentPostViewComponent(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _context.Posts
            .Where(p => p.IsDeleted != true && p.ApprovalStatus == ApprovalStatuses.PUBLISHED)
            .Where( p => (p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)|| p.EndDate == null || p.StartDate == null)
            .OrderByDescending(p => p.CreatedAt)
            .Take(10).ToListAsync();
            return await Task.FromResult((IViewComponentResult)View("Default", model));
        }
    }
}
