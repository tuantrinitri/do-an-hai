using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.MenuManager.DTOs
{
    public class MenuItemTree
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int MenuId { get; set; }

        public string Title { get; set; }

        public string Link { get; set; }

        public string OpenType { get; set; }

        public int Order { get; set; }

        public bool Published { get; set; }

        public string Prefix { get; set; }

        public string TitleWithPrefix { get; set; }

        public IEnumerable<MenuItemTree> Children { get; set; }
    }
}
