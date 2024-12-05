namespace PumpPalace.Models
{
    public class ProductDetailsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal VAT { get; set; }
        public string Currency { get; set; }
        public string PictureUrl { get; set; }
        public int InStock { get; set; }
        public bool IsNew { get; set; }
        public bool IsPromotion { get; set; }
    }
}
