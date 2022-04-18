using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.GalleryManager.DTOs
{
    public class GalleryItemForEditDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        [MaxLength(255, ErrorMessage = "Tiêu đề vượt quá 255 ký tự")]
        public string Title { get; set; }

        [MaxLength(90, ErrorMessage = "Tên file hình ảnh vượt quá 90 ký tự")]
        public string Image { get; set; }

        public string Video { get; set; }

        public bool Published { get; set; }
    }
}
