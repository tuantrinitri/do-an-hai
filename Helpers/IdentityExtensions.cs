using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Principal;

namespace CMS.Helpers
{
    public static class IdentityExtensions
    {
        public static string GetFullName(this IPrincipal usr)
        {
            var fullNameClaim = ((ClaimsIdentity)usr.Identity).FindFirst("FullName");
            if (fullNameClaim != null)
                return fullNameClaim.Value;
            return "";
        }

        public static int GetId(this IPrincipal user)
        {
            var idClaim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim != null)
                return Int32.Parse(idClaim.Value);

            return 0;
        }
        public static bool IsInUnit(this IPrincipal user, string unit)
        {
            var unitClaim = ((ClaimsIdentity)user.Identity).FindFirst("Unit");
            if (unitClaim.Value == unit)
                return true;
            return false;
        }
    }
}
