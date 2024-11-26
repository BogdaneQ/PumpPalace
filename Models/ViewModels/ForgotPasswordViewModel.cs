using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Adres email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public string Email { get; set; }
    }
}
