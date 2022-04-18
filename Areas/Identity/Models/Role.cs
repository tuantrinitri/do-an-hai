using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Identity.Models
{
    public class Role : IdentityRole<int>
    {
        public string Title { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
