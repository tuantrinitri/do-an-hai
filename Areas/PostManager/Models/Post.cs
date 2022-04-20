using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.Identity.Models;
using System.ComponentModel;

namespace CMS.Areas.PostManager.Models
{
    /// <summary>
    /// Model của bài viết
    /// </summary>
    public class Post
    {
        [Display(Name = "Mã bài viết")]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề ")]
        public string Title { get; set; }

        [Display(Name = "Liên kết tĩnh")]
        public string Slug { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string ShortDesc { get; set; }

        [Display(Name = "Nội dung")]
         [DataType(DataType.Html)]
        public string Content { get; set; }

        [Display(Name = "Từ khóa tìm kiếm")]
        public string Keyword { get; set; }

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

        [Display(Name = "Tin nổi bật")]
        [DefaultValue(false)]
        public bool IsFeatured { get; set; }

        // created properties
        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        public User CreatedBy { get; set; }

        public int? CreatedById { get; set; }

        // updated properties
        public DateTime UpdatedAt { get; set; }

        public int? UpdatedById { get; set; }

        public User UpdatedBy { get; set; }

        // approval properties
        [Display(Name = "Trạng thái duyệt")]
        public string ApprovalStatus { get; set; }

        [Display(Name = "Ngày phê duyệt")]
        public DateTime? ApprovalAt { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày hết hạn")]
        public DateTime? EndDate { get; set; }

        public int? ApprovalById { get; set; }

        public User ApprovalBy { get; set; }

        public string ApprovalReason { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [Display(Name = "Chuyên mục")]
        public ICollection<JoinPostCategory> JoinPostCategories { get; set; }

    }
}
