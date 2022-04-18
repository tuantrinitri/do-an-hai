using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.HomepageManager.DTOs
{
    public class UpdateHomepageItemDTO
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Đường dẫn liên kết")]
        public string Slug { get; set; }

        [Display(Name = "Loại liên kết")]
        public string SlugType { get; set; }

        [Display(Name = "Số bài viết hiển thị")]
        public int NoPosts { get; set; }

        [Display(Name = "Thứ tự hiển thị hiển thị")]
        public int Order { get; set; }

        [Display(Name = "Xem trước hình ảnh")]
        public string Type { get; set; }

        [Display(Name = "Hoạt động")]
        public bool Published { get; set; }

        public int BlockId { get; set; }

    }
}
