using System.ComponentModel.DataAnnotations;

namespace TestApp.Models
{
    public class AuthenticateRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
