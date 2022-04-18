using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.NotificationManager.Models
{
    public class Notification
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        public string Title { get; set; }
       

        [Display(Name = "Trạng thái")]
        [DataType(DataType.Text)]
        public string Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày đăng")]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "Người đăng")]
        public string CreatedBy { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sửa")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Name = "Ngày sửa")]
        public string LastModifiedBy { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
