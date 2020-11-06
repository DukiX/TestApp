using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TestApp.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Image { get; set; }
    }
}