using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PumpPalace.Models
{
    public enum OrderStatus
    {
        New,
        Processing,
        Paid,
        Completed,
        Canceled,
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.New;

        [ForeignKey(nameof(Customer))]
        [Required]
        public int CustomerId { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public decimal TotalNetPrice { get; set; }
        public decimal TotalVAT { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue)]
        public decimal VATAmount { get; set; }
    }
}
