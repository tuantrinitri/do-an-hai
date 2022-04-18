using CMS.Areas.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User>
    {
        private readonly UserManager<User> _userManager;

        public CustomClaimsPrincipalFactory(UserManager<User> userManager,
                                            IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
        }
        public async override Task<ClaimsPrincipal> CreateAsync(User user)
        {
            var principal = await base.CreateAsync(user);
            user = _userManager.Users.Include(u => u.Unit).FirstOrDefault(u => u.Id == user.Id);
            // Add your claims here
            var claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            claims.Add(new Claim("Fullname", user.Fullname));
            claims.Add(new Claim("Unit", user.Unit.ShortName));
            ((ClaimsIdentity)principal.Identity).AddClaims(claims);
            return principal;
        }
    }
}
