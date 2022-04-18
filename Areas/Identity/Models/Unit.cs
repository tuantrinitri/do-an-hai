using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.Models
{
    public class Unit
    {
        public int Id { get; set; }

        public string ShortName { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool Activated { get; set; }
    }
}
