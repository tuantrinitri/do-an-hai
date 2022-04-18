using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.DTOs
{
    public class UserForEditDTO
    {
        [Required(ErrorMessage = "Không tìm thấy ID cần sửa")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Chưa nhập họ tên")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Chưa chọn đơn vị")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "Chưa chọn chức vụ")]
        public int JobTitleId { get; set; }

        [Required(ErrorMessage = "Chưa nhập tài khoản")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool Activated { get; set; }

        [Required(ErrorMessage = "Chưa chọn phân quyền")]
        public int RoleId { get; set; }
    }
}
