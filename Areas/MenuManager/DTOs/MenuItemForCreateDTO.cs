using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.MenuManager.DTOs
{
    public class MenuItemForCreateDTO
    {
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        [MaxLength(50, ErrorMessage = "Tiêu đề vượt quá 50 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Chưa nhập liên kết")]
        public string Link { get; set; }

        public string OpenType { get; set; }

        public bool Published { get; set; }
    }
}
