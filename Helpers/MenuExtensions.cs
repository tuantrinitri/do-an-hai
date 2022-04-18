using CMS.Areas.MenuManager.DTOs;
using CMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Helpers
{
    public class MenuExtensions
    {
        private readonly ApplicationDbContext _context;

        public MenuExtensions(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<MenuItemTree> TreeMenu(int? menuId)
        {
            var groups = _context.MenuItems
                .Where(i => i.MenuId == menuId)
                .OrderBy(x => x.Order)
                .ToLookup(x => x.ParentId, x => new MenuItemTree
                {
                    Id = x.Id,
                    Title = x.Title,
                    Link = x.Link,
                    ParentId = x.ParentId,
                    MenuId = x.MenuId,
                    Published = x.Published,
                    OpenType = x.OpenType,
                    Order = x.Order
                });

            // Assign children
            foreach (var item in groups.SelectMany(x => x))
            {
                item.Children = groups[item.Id].ToList();
            }
            return groups[0].ToList();
        }
    }
}
