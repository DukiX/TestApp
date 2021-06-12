using System.ComponentModel.DataAnnotations;
using TestApp.DB;

namespace TestApp.Models
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Try again!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        public string Role { get; set; } = UserRoles.Kupac;
    }
}
