using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.MenuManager.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public int MenuId { get; set; }

        public string Title { get; set; }

        public string Link { get; set; }

        public string OpenType { get; set; }

        public int Order { get; set; }

        public bool Published { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public Menu Menu { get; set; }
    }
}
