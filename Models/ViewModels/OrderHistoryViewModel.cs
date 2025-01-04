namespace PumpPalace.Models
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class OrderHistoryFilterViewModel
    {
        public int? OrderId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public OrderStatus? Status { get; set; }
    }
    public class OrderHistoryListViewModel
    {
        public List<OrderHistoryViewModel> Orders { get; set; } = new List<OrderHistoryViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public OrderHistoryFilterViewModel Filters { get; set; } = new OrderHistoryFilterViewModel();
    }
}
