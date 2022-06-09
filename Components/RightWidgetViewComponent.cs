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
    public class RightWidgetViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public RightWidgetViewComponent(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // get list of widget
            var widgets = await _context.RightWidgets.OrderBy(w => w.Priority).ToListAsync();

            var posts = _context.Posts
            .Where(p => p.IsDeleted != true)
            .Where( p => (p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)|| p.EndDate == null || p.StartDate == null);

            var model = new List<RightWidgetViewModel>();
            foreach(var item in widgets)
            {
                RightWidgetViewModel widget = new RightWidgetViewModel() {
                    Name= item.Name,
                    CategorySlug = item.CategorySlug,
                    IsPreview = item.IsPreview,
                    Posts = item.CategorySlug == null ? posts.Take(item.NoPosts).OrderBy(p => p.IsFeatured).OrderByDescending(p => p.CreatedAt).ToList()
                          : posts.Where(p => p.JoinPostCategories.Any(j => j.PostCategory.Slug == item.CategorySlug)).OrderBy(p => p.IsFeatured).OrderByDescending(p => p.CreatedAt).Take(item.NoPosts).ToList()
                };
                model.Add(widget);
            }

            return await Task.FromResult((IViewComponentResult)View("Default", model));
        }
    }
}
