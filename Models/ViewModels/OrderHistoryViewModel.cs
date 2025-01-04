namespace PumpPalace.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class OrderHistoryListViewModel
    {
        public List<OrderHistoryViewModel> Orders { get; set; } = new List<OrderHistoryViewModel>();
        public int CurrentPage { get; set; } // Obecna strona
        public int TotalPages { get; set; } // Łączna liczba stron
    }
}
