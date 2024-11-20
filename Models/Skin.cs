using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class Skin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string CssFilePath { get; set; }
    }
}
