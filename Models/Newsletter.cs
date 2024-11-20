using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class Newsletter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime ScheduledSendDate { get; set; }

        public bool IsSent { get; set; }
    }
}
