using System.ComponentModel.DataAnnotations;

namespace SmartMentorLive.Api.Models.Users.Requests
{
    public class RegisterUserRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6,ErrorMessage = "Password must be atleast 6 characters")]
        public string Password { get; set; }
        public string Role { get; set; } // Student or Mentor

    }
}
