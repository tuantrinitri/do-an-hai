using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.StaticPages.DTOs
{
    public class UpdatePageDTO
    {
        [Required(ErrorMessage = "Không tìm thấy ID")]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        [MaxLength(255, ErrorMessage = "Tiêu đề vượt quá 255 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [Required(ErrorMessage = "Chưa nhập mô tả ngắn")]
        [MaxLength(255, ErrorMessage = "Mô tả ngắn vượt quá 255 ký tự")]
        [DataType(DataType.Text)]
        public string ShortDesc { get; set; }

        [Display(Name = "Liên kết tĩnh")]
        public string Slug { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; }

        [Display(Name = "Nội dung chi tiết")]
        [DataType(DataType.Html)]
        public string Content { get; set; }

        public bool? IsVisible { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
