using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.PostManager.Models;

namespace CMS.Areas.WidgetManager.DTOs
{
    public class CreateRightWidgetDTO
    {

        [Display(Name = "Tiêu đề")]
        public string Name { get; set; }

        [Display(Name = "Số bài viết hiển thị")]
        public int NoPosts { get; set; }

        [Display(Name = "Thứ tự hiển thị hiển thị")]
        public int Priority { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Hoạt động")]
        public bool Activated { get; set; }

        [Display(Name = "Xem trước hình ảnh")]
        public bool IsPreview { get; set; }

        [Display(Name = "Chuyên mục hiển thị")]
        public string CategorySlug { get; set; }

        [Display(Name = "Chuyên mục hiển thị")]
        public PostCategory Category { get; set; }
    }
}
