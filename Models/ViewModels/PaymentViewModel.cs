namespace PumpPalace.Models
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string CreditCardNumber { get; set; } // Jeśli płatność kartą
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public bool PaymentConfirmed { get; set; } // Status potwierdzenia płatności
    }
}
