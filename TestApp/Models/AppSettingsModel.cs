namespace TestApp.Models
{
    public class AppSettingsModel
    {
        public string JwtValidAudience { get; set; }
        public string JwtValidIssuer { get; set; }
        public string JwtSecret { get; set; }
        public int AccessTokenDurationMinutes { get; set; }
        public int RefreshTokenDurationDays { get; set; }
        public int NumberOfRefreshTokensPerUser { get; set; }
    }
}
