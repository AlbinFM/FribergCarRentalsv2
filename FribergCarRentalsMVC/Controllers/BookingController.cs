using FribergCarRentalsMVC.Data;
using FribergCarRentalsMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsMVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // Visa formulär för att skapa en bokning
        public IActionResult CreateBooking(int carId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Customer");

            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId.Value);
            if (customer == null)
                return RedirectToAction("Login", "Customer");

            ViewBag.CarId = new SelectList(_context.Cars, "Id", "Brand", carId);

            var booking = new Booking
            {
                CarId = carId,
                CustomerId = customerId.Value,
                Customer = customer
            };
            return View(booking);
        }

        // Skapa bokning (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(Booking booking)
        {
            if (ModelState.IsValid)
            {
                if (booking.StartDate > booking.EndDate)
                {
                    ModelState.AddModelError(string.Empty, "Slutdatum måste vara efter startdatum.");
                    ViewBag.CarId = new SelectList(_context.Cars, "Id", "Brand", booking.CarId);
                    ViewBag.CustomerId = booking.CustomerId;
                    return View(booking);
                }

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction("Confirmation");
            }

            ViewBag.CarId = new SelectList(_context.Cars, "Id", "Brand", booking.CarId);
            ViewBag.CustomerId = booking.CustomerId;
            return View(booking);
        }

        // Visa bekräftelsesida efter bokning
        public IActionResult Confirmation()
        {
            return View();
        }

        // Ta bort en bokning
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bokning avbokad!";
                var customerId = booking.CustomerId;
                return RedirectToAction("MyBookings", new { customerId });
            }

            return NotFound();
        }

        // Bekräfta en bokning (admin)
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                booking.IsConfirmed = true;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bokning bekräftad!";
            }

            return RedirectToAction("Bookings", "Admin");
        }

        // Visa alla bokningar för en kund
        public async Task<IActionResult> MyBookings(int customerId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.Car)
                .ToListAsync();

            return View(bookings);
        }
    }
}