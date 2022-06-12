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
using System.Data;

namespace CMS
{
    public class Startup
    {
        private static string WebRootPath { get; set; }

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


        /// <summary>
        /// Đây là method gọi các hàm runtime tiêm vào Program.cs.
        /// Dưới đây là các services được cài từ Nuget Package
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // hàm kết nối với sql server từ applcations.json
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            // services kết nối entity framwork từ Nuget package
            // User, role, permission
            services.AddIdentity<User, Role>(x =>
            {
                x.Password.RequireDigit = false; // cần có ký tự đặc biệt không
                x.Password.RequiredLength = 6; // độ dài bắt buộc mk 
                x.Password.RequireNonAlphanumeric = false; // cần check có chữ không 
                x.Password.RequireUppercase = false; //có yêu cầu có chữ hoa trong mk không
                x.Password.RequireLowercase = false; // có yêu cầu có chữ thường trong mk không
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();
            // service cookie dùng lưu phiên đăng nhập của người dùng
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityTCU.Cookie";
                config.LoginPath = "/Auth/Login";
                config.AccessDeniedPath = "/Auth/AccessDenied";
            });

            // service của .NET6 cho MVC
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
            // thanh điều hướng từ trang chủ tới các trang con trong hệ thống
            // EX: home/tin-tuc/trường sĩ quan thông tin
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
            // services so khớp dữ liệu từ view -> model 
            services.AddAutoMapper(typeof(Startup));
            //----- Render các lớp hỗ trợ cho hệ thống or lập trình ngắn gọn 
            services.AddTransient<ContextHelper>();

            // Render ra menu ngoài trang public
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
            // lấy file hệ thống từ wwwroot
            app.UseStaticFiles();

            app.UseRouting();
            // khai báo với hệ thống người dùng đăng nhập
            app.UseAuthentication();
            // khai báo với hệ thống người dùng đăng nhập sẽ được làm gif
            app.UseAuthorization();
            // khai báo các router cho hệ thống
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
