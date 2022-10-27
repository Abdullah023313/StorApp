using System.ComponentModel.DataAnnotations;

namespace StorApp.Model.UserManager
{
    public class Register
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }= null!;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; } = null!;

    }
}
