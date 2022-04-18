using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.PostManager.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CMS.Areas.PostManager.DTOs
{
    public class UpdatePostDTO
    {
        [Required(ErrorMessage = "Không tìm thấy ID tài liệu để sửa")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Chưa nhập tiêu đề bài viết")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string ShortDesc { get; set; }

        [Required(ErrorMessage = "Chưa nhập nội dung bài viết")]
        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        [Display(Name = "Từ khóa tìm kiếm")]
        public string Keyword { get; set; }

        public string ApprovalReason { get; set; }

        [Display(Name = "Tác giả")]
        public string Author { get; set; }

        [Display(Name = "Nguồn tin")]
        public string InfoSource { get; set; }

        [Display(Name = "Hình ảnh minh họa")]
        public string AvatarUrl { get; set; }

        [Display(Name = "Tiêu đề ảnh đại diện")]
        public string AvatarCaption { get; set; }

        [Display(Name = "Văn bản thay thế ảnh đại diện")]
        public string AvatarAltText { get; set; }

        [Display(Name = "Tệp đính kèm")]
        public string FileAttachments { get; set; }

        [Display(Name = "Trạng thái duyệt")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Danh mục")]
        public List<SelectPostCategoryDTO> PostCategories { get; set; }

        [Display(Name = "Tin nổi bật")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Ngày đăng")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày hết hạn")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndDate { get; set; }

    }
}
