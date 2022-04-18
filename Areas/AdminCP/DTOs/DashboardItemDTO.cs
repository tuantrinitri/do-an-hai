using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.AdminCP.DTOs
{
    public class DashboardItemDTO
    {
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Route { get; set; }
        public string IconClass { get; set; }
        public string Title { get; set; }
        public string Total { get; set; }
        public string BgColorClass { get; set; }
    }
}
