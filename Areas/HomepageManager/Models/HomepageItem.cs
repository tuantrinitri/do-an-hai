using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.HomepageManager.Models
{
    public class HomepageItem
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề khối")]
        public string Title { get; set; }

        [Display(Name = "Đường dẫn liên kết")]
        public string Slug { get; set; }

        [Display(Name = "Loại liên kết")]
        public string SlugType { get; set; }

        [Display(Name = "Số bài viết hiển thị")]
        [DefaultValue(5)]
        public int NoPosts { get; set; }

        [Display(Name = "Thứ tự hiển thị hiển thị")]
        [DefaultValue(5)]
        public int Order { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Hoạt động")]
        public bool Published { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Xem trước hình ảnh")]
        public bool IsPreview { get; set; }

        [Display(Name = "Loại mục")]
        public string Type { get; set; }

        public int BlockId { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public HomepageBlock Block { get; set; }
    }
}
