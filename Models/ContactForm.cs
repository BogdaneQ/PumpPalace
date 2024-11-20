using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ContactForm
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime SubmittedAt { get; set; }
    }
}
