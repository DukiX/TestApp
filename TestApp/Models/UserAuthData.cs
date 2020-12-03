namespace TestApp.Models
{
    public class UserAuthData
    {
        public string AccessToken { get; set; }
        public string ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public string Issued { get; set; }
        public string Expires { get; set; }
        public string UserRole { get; set; }
        public string UserEmail { get; set; }
    }
}
