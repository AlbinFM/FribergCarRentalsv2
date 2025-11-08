using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var bookedCarIds = await _context.Bookings
                .Where(b => b.IsConfirmed && b.EndDate >= DateTime.Today)
                .Select(b => b.CarId)
                .ToListAsync();

            var availableCars = await _context.Cars
                .Where(c => !bookedCarIds.Contains(c.Id))
                .ToListAsync();

            return View(availableCars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> CarDetailsAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
    }
}