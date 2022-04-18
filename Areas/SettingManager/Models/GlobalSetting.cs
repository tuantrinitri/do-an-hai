using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Areas.SettingManager.Models
{
    public class GlobalSetting
    {
        public int Id { get; set; }

        public string URL { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên đơn vị")]
        public string UnitName { get; set; }

        [Required(ErrorMessage = "Chưa nhập tên tiếng Anh")]
        public string UnitNameEn { get; set; }

        [Required(ErrorMessage = "Chưa chọn hình ảnh")]
        public string LogoHeader { get; set; }

        [Required(ErrorMessage = "Chưa chọn hình ảnh")]
        public string LogoFooter { get; set; }

        [Required(ErrorMessage = "Chưa nhập website")]
        public string SiteType { get; set; }

        //[DataType(DataType.EmailAddress, ErrorMessage = "Email chưa đúng định dạng")]
        [RegularExpression(@"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*;\s*|\s*$))+",ErrorMessage = "Email chưa đúng định dạng")]
        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Fax { get; set; }

        public string MoreInfo { get; set; }
        public string EmbedMapURL { get; set; }

        public string ScriptChatBot { get; set; }
        public string ScriptGoogleTag { get; set; }

        public DateTime LastModifiedAt { get; set; }

    }
}
