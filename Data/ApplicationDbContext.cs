using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CMS.Areas.PostManager.Models;
using CMS.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using CMS.Areas.StaticPages.Models;
using CMS.Areas.NotificationManager.Models;
using CMS.Areas.SettingManager.Models;
using CMS.Areas.ContactManager.Models;
using CMS.Areas.GalleryManager.Models;
using CMS.Areas.MenuManager.Models;
using CMS.Areas.WidgetManager.Models;
using CMS.Areas.HomepageManager.Models;

namespace CMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, int,
        IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<JoinPostCategory> JoinPostCategories { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<StaticPage> StaticPages { get; set; }


        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }


        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<GlobalSetting> GlobalSettings { get; set; }

        public DbSet<RightWidget> RightWidgets { get; set; }

        public DbSet<HomepageBlock> HomepageBlocks { get; set; }

        public DbSet<HomepageItem> HomepageItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new {ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            builder.Entity<JoinPostCategory>(postCat =>
            {
                postCat.HasKey(pc => new {pc.PostId, pc.CategoryId});
                postCat.HasOne(pc => pc.PostCategory)
                    .WithMany(c => c.JoinPostCategories)
                    .HasForeignKey(pc => pc.CategoryId)
                    .IsRequired();
                postCat.HasOne(pc => pc.Post)
                    .WithMany(c => c.JoinPostCategories)
                    .HasForeignKey(pc => pc.PostId)
                    .IsRequired();
            });
        }
    }
}