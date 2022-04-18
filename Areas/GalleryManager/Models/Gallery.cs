using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.GalleryManager.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Alias { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public bool Published { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
