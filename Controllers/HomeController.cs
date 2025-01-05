using Microsoft.AspNetCore.Mvc;
using PumpPalace.Models;

namespace PumpPalace.Controllers
{
    public class HomeController : Controller
    {
        private readonly PumpPalaceDbContext _context;

        public HomeController(PumpPalaceDbContext dbContext)
        {
            _context = dbContext;
        }
        public IActionResult Index()
        {
            var counter = _context.ViewsCounters.FirstOrDefault();

            if (counter == null)
            {
                // Jeśli brak rekordu, utwórz nowy
                counter = new ViewsCounter { Counter = 1 };
                _context.ViewsCounters.Add(counter);
            }
            else
            {
                // Zwiększ licznik
                counter.Counter = (counter.Counter ?? 0) + 1;
            }

            _context.SaveChanges(); // Zapisz zmiany do bazy danych

            // Przekaż wartość do widoku
            ViewBag.PageViews = counter.Counter;

            return View();
        }


        public IActionResult Error()
        {
            return View();
        }
    }
}
