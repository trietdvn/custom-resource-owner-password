using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Test;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class ProfileService : IProfileService
    {
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();

            var user = Config.GetUsers().FirstOrDefault(p => p.SubjectId == sub);
            var cp = GetClaims(user);

            var claims = cp.Claims;
            if (context.RequestedClaimTypes != null && context.RequestedClaimTypes.Any())
            {
                claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToArray().AsEnumerable();
            }

            context.IssuedClaims = claims.ToList();
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }

        private ClaimsPrincipal GetClaims(TestUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var id = new ClaimsIdentity();
            id.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, user.Username));
            id.AddClaim(new Claim(JwtClaimTypes.Subject, user.SubjectId));

            return new ClaimsPrincipal(id);
        }
    }
}
