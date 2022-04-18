using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.StaticPages.Models
{
    public class StaticPage
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        [MaxLength(255, ErrorMessage = "Tiêu đề vượt quá 255 ký tự")]
        public string Title { get; set; }

        [Display(Name = "Liên kết tĩnh")]
        public string Slug { get; set; }

        [Display(Name = "Hình ảnh minh họa")]
        public string FeaturedImage { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [DataType(DataType.Text)]
        [MaxLength(255, ErrorMessage = "Mô tả ngắn vượt quá 255 ký tự")]
        public string ShortDesc { get; set; }

        [Display(Name = "Nội dung chi tiết")]
        [Required(ErrorMessage = "Chưa nhập nội dung chi tiết")]
        [DataType(DataType.Html)]
        public string Content { get; set; }

        [Display(Name = "Trạng thái")]
        [DataType(DataType.Text)]
        public string Status { get; set; }

        [Display(Name = "Trạng thái")]
        [DataType(DataType.Text)]
        public bool? IsVisible { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Thời gian đăng")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Đăng bởi")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sửa")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Người sửa")]
        public string LastModifiedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
