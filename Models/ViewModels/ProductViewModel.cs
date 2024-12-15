namespace PumpPalace.Models.ViewModels
{
    public class ProductViewModel
    {
        public Product CurrentProduct { get; set; }
        public IEnumerable<Product> ProductList { get; set; }
    }

}
