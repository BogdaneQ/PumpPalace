namespace PumpPalace.Models
{
    public class WishlistItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal Price { get; set; }
        public bool IsInStock { get; set; } // Status dostępności produktu
    }

    public class WishlistViewModel
    {
        public List<WishlistItemViewModel> Items { get; set; } // Lista elementów na liście życzeń
    }
}
