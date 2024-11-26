using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ResetPasswordViewModel
    {
        public string Token { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Compare("Password", ErrorMessage = "Hasła muszą być takie same.")]
        public string ConfirmPassword { get; set; }
    }

}
