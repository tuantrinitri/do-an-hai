using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CMS.Models
{
    public class HeaderBarViewModel
    {

        public string URL { get; set; }

        public string UnitName { get; set; }

        public string UnitNameEn { get; set; }

        public string LogoHeader { get; set; }
        public string LogoFooter { get; set; }

        public string SiteType { get; set; }
   
        public string Email { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Fax { get; set; }

        public string MoreInfo { get; set; }
        public string EmbedMapURL { get; set; }

        public DateTime LastModifiedAt { get; set; }

    }
}
