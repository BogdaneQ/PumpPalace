using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public class ProductStats
    {
        [Key, ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int PurchaseCount { get; set; }

        [Range(0, int.MaxValue)]
        public int ViewCount { get; set; }

    }
}
