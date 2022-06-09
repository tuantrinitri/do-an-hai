using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.Models
{
    public class User : IdentityUser<int>
    {
        public string Fullname { get; set; }

        public bool Activated { get; set; }

        public bool FirstLogin { get; set; }

        public string Role { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
