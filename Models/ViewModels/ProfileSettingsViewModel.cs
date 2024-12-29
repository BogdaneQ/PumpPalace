using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ProfileSettingsViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Podaj poprawny numer telefonu")]
        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
