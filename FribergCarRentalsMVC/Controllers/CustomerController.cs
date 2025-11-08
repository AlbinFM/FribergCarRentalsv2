using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    public class CustomerController : Controller
    {
                private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        // Visa registreringsformulär
        public IActionResult CustomerRegister()
        {
            return View();
        }

        // Skapa kund (POST)
        [HttpPost]
        public async Task<IActionResult> CustomerRegister(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("CustomerLogin");
            }
            return View(customer);
        }

        // Visa inloggningsformulär
        public IActionResult CustomerLogin()
        {
            return View();
        }

        // Hantera inloggning (POST)
        [HttpPost]
        public async Task<IActionResult> CustomerLogin(string email, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == email && c.Password == password);

            if (customer == null)
            {
                ViewBag.Error = "Fel e-post eller lösenord";
                return View();
            }

            HttpContext.Session.SetInt32("CustomerId", customer.Id);
            return RedirectToAction("MyBookings");
        }

        // Logga ut kund
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("CustomerLogin");
        }

        // Visa alla bokningar för inloggad kund
        public async Task<IActionResult> MyBookingsAsync()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("CustomerLogin");

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();

            return View(bookings);
        }
    }
}