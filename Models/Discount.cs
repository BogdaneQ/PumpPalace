using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Customer))]
        public int UserId { get; set; }

        [Required]
        public decimal Percentage { get; set; }

        public DateTime? ExpiryDate { get; set; }

    }
}
