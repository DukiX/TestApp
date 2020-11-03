using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TestApp.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}