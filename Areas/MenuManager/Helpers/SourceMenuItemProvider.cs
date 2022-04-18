using CMS.Areas.MenuManager.DTOs;
using CMS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.MenuManager.Helpers
{
    public class SourceMenuItemProvider
    {
        private readonly ApplicationDbContext _context;

        public SourceMenuItemProvider(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<SourceItem>> PostCategoriesAsync()
        {
            List<SourceItem> result = new List<SourceItem>();

            var postCategories = await _context.PostCategories.ToListAsync();
            foreach (var postCategory in postCategories)
            {
                result.Add(new SourceItem
                {
                    Title = postCategory.Name,
                    Link = "/chuyen-muc/" + postCategory.Slug
                });
            }

            return result;
        }

        public async Task<List<SourceItem>> StaticPageAsync()
        {
            List<SourceItem> result = new List<SourceItem>();

            var pages = await _context.StaticPages.ToListAsync();
            foreach (var page in pages)
            {
                result.Add(new SourceItem
                {
                    Title = page.Title,
                    Link = "/trang/" + page.Slug
                });
            }

            return result;
        }

        public List<SourceItem> OtherItem()
        {
            List<SourceItem> result = new List<SourceItem>();

            result.Add(new SourceItem
            {
                Title = "Liên hệ",
                Link = "/lien-he"
            });
            result.Add(new SourceItem
            {
                Title = "Câu hỏi thường gặp",
                Link = "/cau-hoi-thuong-gap"
            });

            return result;
        }
    }
}
