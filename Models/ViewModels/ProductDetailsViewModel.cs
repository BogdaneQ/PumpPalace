namespace PumpPalace.Models
{
    public class ProductDetailsViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public bool IsInStock { get; set; }
        public List<string> Categories { get; set; }
        public List<ReviewViewModel> Reviews { get; set; } // Recenzje produktu
    }

    public class ReviewViewModel
    {
        public string ReviewerName { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}
