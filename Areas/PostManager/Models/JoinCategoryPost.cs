using CMS.Areas.PostManager.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.Models
{
    /// <summary>
    /// Convert table => join 2 bảng Pots và Category
    /// Phục vụ 1 bài post có nhiều category
    /// </summary>
    public class JoinPostCategory
    {
        public Post Post { get; set; }

        public int PostId { get; set; }

        public PostCategory PostCategory { get; set; }

        public int CategoryId { get; set; }
    }
}
