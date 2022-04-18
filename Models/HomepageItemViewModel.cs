using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using CMS.Areas.PostManager.Models;

namespace CMS.Models
{
    public class HomepageItemViewModel
    {
        public string Title { get; set; }

        public string CategorySlug {get; set;}

        public string Type { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
