using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.PostManager.DTOs
{
    public class PartialMenuDTO
    {
        public string MiController { get; set; }

        public string MiAction { get; set; }

        public string MiRoute { get; set; }

        public string MiTitle { get; set; }

        public int? MiTotal { get; set; }

        public string MiColor { get; set; }
    }
}
