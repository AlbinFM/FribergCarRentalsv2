using FribergCarRentalsMVC.Data;
using FribergCarRentalsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsMVC.Controllers
{
    public class AdminController : Controller
    {
                private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Visa inloggningsformulär för admin
        [HttpGet]
        public IActionResult Index() => RedirectToAction("AdminLogin");

        [HttpGet]
        public IActionResult AdminLogin() => View();

        // Hantera inloggning för admin
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email && a.Password == password);
            if (admin == null)
            {
                ViewBag.LoginFailed = true;
                return View("AdminLogin");
            }
            HttpContext.Session.SetInt32("AdminId", admin.Id);
            return RedirectToAction("Dashboard");
        }

        // Logga ut admin
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            return RedirectToAction("AdminLogin");
        }

        // Visa admin-dashboard
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");
            return View();
        }

        // Visa alla bilar
        public async Task<IActionResult> Cars()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var cars = await _context.Cars.ToListAsync();
            return View(cars);
        }

        // Visa formulär för att skapa bil
        public IActionResult CreateCar()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");
            return View();
        }

        // Skapa bil (POST)
        [HttpPost]
        public async Task<IActionResult> CreateCar(Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Cars.Add(car);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Annons skapad!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index", "Cars");
            }
            return View(car);
        }

        // Redigera bil (GET)
        public async Task<IActionResult> EditCar(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();
            return View(car);
        }

        // Redigera bil (POST)
        [HttpPost]
        public async Task<IActionResult> EditCar(Car car, string ImageUrlsString)
        {
            if (ModelState.IsValid)
            {
                car.ImageUrls = ImageUrlsString?
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) //Efter man redigerade en bil så pekade annonsen på listan med bilder istälet för URL:n, så hittade denna kod på nätet som konverterar URL:n till en lista innan den sparas.
                    .ToList() ?? new List<string>();

                _context.Cars.Update(car);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Annons uppdaterad!";
                TempData["AlertType"] = "warning";
                return RedirectToAction("Index", "Cars");
            }
            return View(car);
        }

        // Ta bort bil (GET)
        public async Task<IActionResult> DeleteCar(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();
            return View(car);
        }

        // Ta bort bil (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteCarConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Annons borttagen!";
                TempData["AlertType"] = "danger";
            }
            return RedirectToAction("Index", "Cars");
        }

        // Visa alla kunder
        public async Task<IActionResult> Customers()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var customers = await _context.Customers.ToListAsync();
            return View(customers);
        }

        // Visa detaljer för en kund
        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Car)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Visa bekräftelse för att ta bort kund (GET)
        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound();

            return View(customer);
        }

        // Ta bort kund (POST)
        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer != null)
            {
                if (customer.Bookings != null && customer.Bookings.Any())
                {
                    _context.Bookings.RemoveRange(customer.Bookings);
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Kund borttagen!";
                TempData["AlertType"] = "danger";
            }
            return RedirectToAction("Customers");
        }

        // Visa alla bokningar
        public async Task<IActionResult> Bookings()
        {
            if (HttpContext.Session.GetInt32("AdminId") == null)
                return RedirectToAction("AdminLogin");

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .ToListAsync();
            return View(bookings);
        }
    }
}