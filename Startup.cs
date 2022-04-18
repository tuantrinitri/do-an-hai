using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartBreadcrumbs.Extensions;
using CMS.Areas.Identity.Models;
using CMS.Helpers;
using CMS.Models;
using AutoMapper;
using System.IO;

namespace CMS
{
    public class Startup
    {
        private static string WebRootPath { get;  set; }

        public static string MapPath(string path, string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = WebRootPath;
            }

            //path = path.Replace("~/", "").TrimStart('/');

            return Path.Combine(basePath, path);
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 6;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireLowercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityTCU.Cookie";
                config.LoginPath = "/Auth/Login";
                config.AccessDeniedPath = "/Auth/AccessDenied";
            });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();

            services.AddBreadcrumbs(GetType().Assembly, options =>
            {
                options.TagName = "div";
                options.TagClasses = "d-flex";
                options.OlClasses = "breadcrumb";
                options.LiClasses = "breadcrumb-item";
                options.ActiveLiClasses = "breadcrumb-item active";
                options.SeparatorElement = "";
            });
            //-----Config reCAPTCHA v2-v3
            services.Configure<ReCAPTCHASettings_V3>(Configuration.GetSection("GooglereCAPTCHA_V3"));
            services.Configure<ReCAPTCHASettings_V2>(Configuration.GetSection("GooglereCAPTCHA_V2"));
            services.AddTransient<GoogleReCAPTCHAService>();
            //-----Add Auto Mapper
            services.AddAutoMapper(typeof(Startup));
            //-----Test
            services.AddTransient<ContextHelper>();
            services.AddTransient<MenuExtensions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                // context.Database.EnsureDeleted();
                // context.Database.EnsureCreated();
                // SeedData.EnsureSeedData(Configuration.GetConnectionString("DefaultConnection"));
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "trang",
                    defaults: new { controller = "Page", action = "Details" },
                    pattern: "trang/{*slug}");
                endpoints.MapControllerRoute(
                    name: "bai-viet",
                    defaults: new { controller = "Post", action = "Details" },
                    pattern: "bai-viet/{*slug}");
                endpoints.MapControllerRoute(
                    name: "chuyen-muc",
                    defaults: new { controller = "Category", action = "Index" },
                    pattern: "chuyen-muc/{*slug}");
                endpoints.MapControllerRoute(
                    name: "tai-lieu",
                    defaults: new { controller = "Document", action = "Details" },
                    pattern: "tai-lieu/{*slug}");
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");               
                endpoints.MapRazorPages();
            });
            WebRootPath = env.WebRootPath;
        }
    }
}
