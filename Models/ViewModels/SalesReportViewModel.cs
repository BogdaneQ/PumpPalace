using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PumpPalace.Models
{
    public class SalesReportViewModel
    {
        public int TotalSales { get; set; }
        public int OrdersCompleted { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
