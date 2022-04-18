using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CMS.Areas.Identity.Models;
using CMS.Areas.PostManager.Models;

namespace CMS.Areas.WidgetManager.Models
{
    public class RightWidget
    {
        [Display(Name = "Mã widget")]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Name { get; set; }
        public string CategorySlug { get; set; }

        [Display(Name = "Số bài viết hiển thị")]
        [DefaultValue(5)]
        public int NoPosts { get; set; }

        [Display(Name = "Thứ tự hiển thị hiển thị")]
        [DefaultValue(5)]
        public int Priority { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Hoạt động")]
        public bool Activated { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Xem trước hình ảnh")]
        public bool IsPreview { get; set; }
    }
}
