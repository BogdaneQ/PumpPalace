using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class CustomerRegisterViewModel
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Podaj poprawny adres email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [Compare("Password", ErrorMessage = "Hasła muszą być zgodne.")]
        public string ConfirmPassword { get; set; }

        public string Address { get; set; }

        [Phone(ErrorMessage = "Podaj poprawny numer telefonu.")]
        public string Phone { get; set; }

        public bool IsSubscribedToNewsletter { get; set; } = false;
    }
}
