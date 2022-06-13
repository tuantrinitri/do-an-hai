// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
    
 
using CMS.Areas.GalleryManager.Models;
using CMS.Areas.HomepageManager.Models;
using CMS.Areas.Identity.Models;
   
using CMS.Areas.MenuManager.Models;
  
using CMS.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using Slugify;

namespace CMS.Data
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<User, Role>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 6;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>();

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    //context.Database.Migrate();
                    // context.Database.EnsureDeleted();
                    // context.Database.EnsureCreated();
                    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                    SeedRoles(roleMgr);
                    SeedUser(userMgr, context);
                    SeedMenu(context);
                    SeedGalleries(context);
                    SeedHomepageBlocks(context);
                }
            }
        }
        private static void SeedRoles(RoleManager<Role> _roleManager)
        {
            if (!_roleManager.Roles.Any())
            {
                var roles = new List<Role>
                {
                    new Role { Name="admin", Title="Administrator"},
                    new Role { Name="ctv", Title="Cộng tác viên"},
                };

                foreach (var role in roles)
                {
                    var result = _roleManager.CreateAsync(role).Wait(-1);
                    if (result) Log.Debug("Role " + role.Name + " - " + role.Title + " created");
                }
            }
        }
        
        private static void SeedUser(UserManager<User> _userManager, ApplicationDbContext _dataContext)
        {
            if (!_userManager.Users.Any())
            {
                var dataJson = System.IO.File.ReadAllText("Areas/Identity/Data/user.json");
                var users = JsonConvert.DeserializeObject<List<User>>(dataJson);

                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "123456").Wait();
                    _userManager.AddToRoleAsync(user, user.Role).Wait();
                    Log.Debug("User " + user.UserName + " - " + user.Fullname + " created");
                }
            }
        }

        private static void SeedMenu(ApplicationDbContext _dataContext)
        {
            if (!_dataContext.Menus.Any())
            {
                var menus = new List<Menu>
                {
                    new Menu { Alias="MAIN_MENU", Title="MENU đầu trang - menu chính"},
                    new Menu { Alias="FOOTER_MENU_LEFT", Title="MENU cuối trang - Bên trái"},
                    new Menu { Alias="FOOTER_MENU_RIGHT", Title="MENU cuối trang - Bên phải"}
                };

                foreach (var menu in menus)
                {
                    menu.Published = true;
                    _dataContext.Menus.Add(menu);
                }
                _dataContext.SaveChanges();
                Log.Debug("Menus created");
            }
        }

        private static void SeedGalleries(ApplicationDbContext _dataContext)
        {
            if (!_dataContext.Galleries.Any())
            {
                var galleries = new List<Gallery>
                {
                    new Gallery { Alias="SLIDER_HOME", Title="Slider trang chủ", Type="photo"},
                    new Gallery { Alias="GALLERY_PHOTO", Title="Thư viện hình ảnh", Type="photo"},
                    new Gallery { Alias="GALLERY_VIDEO", Title="Thư viện video", Type="video"}
                };

                foreach (var gallery in galleries)
                {
                    gallery.Published = true;
                    gallery.CreatedAt = DateTime.Now;
                    gallery.LastModifiedAt = DateTime.Now;
                    _dataContext.Galleries.Add(gallery);
                }
                _dataContext.SaveChanges();
                Log.Debug("Galleries created");
            }
        }
     
        private static void SeedHomepageBlocks(ApplicationDbContext _dataContext)
        {
            if (!_dataContext.HomepageBlocks.Any())
            {
                var menus = new List<HomepageBlock>
                {
                    new HomepageBlock { Alias="BLOCK_HOME_1", Title="Trang chủ - khối 1", Type="dual"},
                    new HomepageBlock { Alias="BLOCK_HOME_2", Title="Trang chủ - khối 2", Type="dual"},
                    new HomepageBlock { Alias="BLOCK_HOME_3", Title="Trang chủ - khối 3", Type="tripple"},
                    new HomepageBlock { Alias="BLOCK_HOME_4", Title="Trang chủ - khối 4", Type="tripple"},
                };

                foreach (var menu in menus)
                {
                    menu.Published = true;
                    _dataContext.HomepageBlocks.Add(menu);
                }
                _dataContext.SaveChanges();
                Log.Debug("Homepage blocks created");
            }
        }
    }
}
