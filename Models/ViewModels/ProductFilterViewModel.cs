namespace PumpPalace.Models
{
    public class ProductFilterViewModel
    {
        public string SearchTerm { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? OnDiscount { get; set; }
        public bool? InStock { get; set; }

        // Lista przefiltrowanych produktów
        public IEnumerable<Product> Products { get; set; }
    }
}
