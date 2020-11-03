using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace TestApp.Tools
{
    public class TokensHelper
    {
        public static string GetClaimFromJwt(HttpContext httpContext, string claim)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            if (identity == null) return null;

            IEnumerable<Claim> claims = identity.Claims;
            string claimValue = claims.FirstOrDefault(c => c.Type == claim)?.Value;

            return claimValue;
        }
    }
}
