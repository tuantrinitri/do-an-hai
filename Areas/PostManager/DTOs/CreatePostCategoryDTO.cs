using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.PostManager.DTOs
{
    public class CreatePostCategoryDTO
    {
        [Required(ErrorMessage = "Chưa nhập tên chuyên mục")]
        [MaxLength(50, ErrorMessage = "Tên chuyên mục quá dài, tối đa 50 ký tự")]
        [Display(Name = "Tên chuyên mục")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "Mô tả ngắn quá dài, tối đa 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        
        [DefaultValue(null)]
        [Display(Name = "Hình ảnh danh mục")]
        public string ImageCategory { get; set; }
        
        [Display(Name = "Hiển thị")]
        public bool Activated { get; set; }
    }
}
