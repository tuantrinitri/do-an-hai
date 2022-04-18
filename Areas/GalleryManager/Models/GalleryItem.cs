using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.GalleryManager.Models
{
    public class GalleryItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Video { get; set; }

        public bool Published { get; set; }

        public int Order { get; set; }

        public int GalleryId { get; set; }

        public Gallery Gallery { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
