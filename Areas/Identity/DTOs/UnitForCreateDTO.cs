using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.DTOs
{
    public class UnitForCreateDTO
    {
        [Required(ErrorMessage ="Chưa nhập tên viết tắt")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên đơn vị")]
        public string Title { get; set; }

        public string Description { get; set; }

        public bool Activated { get; set; }
    }
}
