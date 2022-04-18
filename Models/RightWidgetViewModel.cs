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
    public class RightWidgetViewModel
    {
        public string Name { get; set; }

        public string CategorySlug {get; set;}

        public bool IsPreview { get; set; }

        public ICollection<Post> Posts { get; set; }

    }
}
