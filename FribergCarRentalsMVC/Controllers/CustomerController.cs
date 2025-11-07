using FribergCarRentalsMVC.Data;
using FribergCarRentalsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public IActionResult Register()
        {
            return View();
        }

        // Skapa kund (POST)
        [HttpPost]
        public async Task<IActionResult> Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(customer);
        }

        // Visa inloggningsformulär
        public IActionResult Login()
        {
            return View();
        }

        // Hantera inloggning (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
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
            return RedirectToAction("Login");
        }

        // Visa alla bokningar för inloggad kund
        public async Task<IActionResult> MyBookings()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();

            return View(bookings);
        }
    }
}