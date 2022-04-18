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
using CMS.Areas.MenuManager;

namespace CMS.Components
{
    public class HeaderBarViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        private readonly MenuExtensions _menuHelper;

        public HeaderBarViewComponent(ApplicationDbContext context, UserManager<User> userManager, IMapper mapper, MenuExtensions menuHelper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _menuHelper = menuHelper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            HeaderBarViewModel setting = null;
            var headerSetting = _context.GlobalSettings.FirstOrDefault();
            if (headerSetting == null)
            {
                setting = new HeaderBarViewModel()
                {
                    Address = "O nha",
                    Email = "",
                    EmbedMapURL = "",
                    SiteType = "",
                    URL = "",
                    UnitName = "",
                    PhoneNumber = ""
                };
            }
            else
            {
                setting = _mapper.Map<HeaderBarViewModel>(headerSetting);
            }
            var top_menu = _context.Menus.Where(m => m.Alias == "MAIN_MENU").First();
            ViewData["MainMenuItems"] = _menuHelper.TreeMenu(top_menu.Id).AsQueryable();
            return await Task.FromResult((IViewComponentResult)View("Default", setting));
        }
    }
}
