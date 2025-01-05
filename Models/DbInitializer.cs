using System.Diagnostics.Metrics;

namespace PumpPalace.Models
{
    public class DbInitializer
    {
        public static void Initialize(PumpPalaceDbContext context)
        {
            if (!context.ViewsCounters.Any())
            {
                context.ViewsCounters.Add(new ViewsCounter { Counter = 0 });

                context.SaveChanges();
            }
        }
    }
}
