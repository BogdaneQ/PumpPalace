namespace PumpPalace.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int Quantity { get; set; }
        public int InStock { get; set; }

        public decimal TotalPrice => (DiscountPrice.HasValue && DiscountPrice < Price)
                                    ? DiscountPrice.Value * Quantity
                                    : Price * Quantity;
    }



}
