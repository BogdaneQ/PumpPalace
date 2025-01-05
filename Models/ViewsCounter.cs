using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ViewsCounter
    {
        [Key]
        public int Id { get; set; }
        public int? Counter { get; set; }
    }
}
