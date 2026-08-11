using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using ParkLink.Identity.Data;
using ParkLink.Identity.Models;
using System.Security.Claims;

namespace ParkLink.Identity.Services
{
    public class PermissionProfileService : IProfileService
    {
        //private readonly MawumsContext _db;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

        public PermissionProfileService(//MawumsContext db,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        {
            _db = db;
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.FindFirst("sub")?.Value;
            if (userId == null) return;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            // base claims from Identity (sub, name, email, role already included)
            var principal = await _claimsFactory.CreateAsync(user);
            var claims = principal.Claims.ToList();

            // add email_verified only if not present
            if (!claims.Any(x => x.Type == "email_verified"))
            {
                claims.Add(new Claim("email_verified",
                    user.EmailConfirmed.ToString().ToLower()));
            }

            //// load permissions
            //var roleIds = await _db.AspNetUserRoles
            //    .Where(x => x.UserId == userId)
            //    .Select(x => x.RoleId)
            //    .ToListAsync();

            //// Add RoleId claim
            //foreach (var roleId in roleIds)
            //{
            //    claims.Add(new Claim("role_id", roleId));
            //}

            //var permissions = await _db.RolePermissions
            //    .Where(rp => roleIds.Contains(rp.RoleId))
            //    .Select(rp => rp.Permission.Code)
            //    .Distinct()
            //    .ToListAsync();

            //foreach (var permission in permissions)
            //    claims.Add(new Claim("permission", permission));

            // IMPORTANT — ensure uniqueness (critical)
            context.IssuedClaims = claims
                .Where(c => context.RequestedClaimTypes.Contains(c.Type)
                            || c.Type == "permission"
                            || c.Type == "role_id")
                .GroupBy(c => new { c.Type, c.Value })
                .Select(g => g.First())
                .ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.FindFirst("sub")?.Value;
            if (userId == null)
            {
                context.IsActive = false;
                return;
            }

            var user = await _userManager.FindByIdAsync(userId);
            context.IsActive = user != null;
        }
    }
}
