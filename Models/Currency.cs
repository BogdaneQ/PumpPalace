using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class Currency
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

    }
}
