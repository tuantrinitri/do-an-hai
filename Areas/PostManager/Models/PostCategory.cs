using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using CMS.Areas.Identity.Models;

namespace CMS.Areas.PostManager.Models
{
    /// <summary>
    /// Model của Chuyên Mục bài viết
    /// </summary>
    public class PostCategory
    {
        [Display(Name = "Mã chuyên mục")]
        public int Id { get; set; }

        [Display(Name = "Tên chuyên mục")]
        public string Name { get; set; }

        [Display(Name = "Liên kết tĩnh")]
        public string Slug { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [DefaultValue(true)]
        [Display(Name = "Hoạt động")]
        public bool Activated { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public ICollection<JoinPostCategory> JoinPostCategories { get; set; }
    }
}
