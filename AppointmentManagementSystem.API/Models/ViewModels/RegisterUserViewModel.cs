using System.ComponentModel.DataAnnotations;


namespace AppointmentManagementSystem.API.Models.ViewModels
{
    public class RegisterUserViewModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
        [MaxLength(20, ErrorMessage = "Username must be at most 20 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9_.-]*$", ErrorMessage = "Username can only contain alphanumeric characters, underscores (_), hyphens (-), and dots (.)")]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
    }
}
