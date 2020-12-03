using System;

namespace TestApp.DB
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
