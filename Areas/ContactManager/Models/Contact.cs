using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.ContactManager.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập tiêu đề")]
        [MaxLength(50, ErrorMessage = "Tiêu đề vượt quá 50 ký tự")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Chưa nhập họ và tên")]
        [MaxLength(50, ErrorMessage = "Họ và tên vượt quá 50 ký tự")]
        [RegularExpression(@"(([a-zA-Z\u00BF-\u1FFF\u2C00-\uD7FF]+)[\s]{0,1})*", ErrorMessage = "Họ tên chưa đúng định dạng")]
        [DataType(DataType.Text)]
        public string Fullname { get; set; }
        
        [Required(ErrorMessage = "Chưa nhập email")]
        [RegularExpression(@"(([a-zA-Z0-9_\-\.][a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))+", ErrorMessage = "Email chưa đúng định dạng")]
        [MaxLength(32, ErrorMessage = "Email vượt quá 32 ký tự")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Chưa nhập số điện thoại")]
        [RegularExpression(@"^[0-9+]+$", ErrorMessage = "Số điện thoại ko hợp lệ")]
        [MaxLength(14, ErrorMessage = "Số điện thoại vượt quá 14 ký tự")]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Chưa nhập nội dung")]
        [MaxLength(255, ErrorMessage = "Nội dung vượt quá 255 ký tự")]
        [DataType(DataType.Text)]
        public string Content { get; set; }

        public bool Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastModifiedAt { get; set; }
    }
}
