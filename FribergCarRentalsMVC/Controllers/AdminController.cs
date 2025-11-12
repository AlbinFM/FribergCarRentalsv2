using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;
using FribergCarRentalsMVC.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly IAdminApiClient _api;
        public AdminController(IAdminApiClient api) => _api = api;
        
        [HttpGet]
        public IActionResult Dashboard() => View();

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        //Cars
        [HttpGet]
        public IActionResult ShowCars() => RedirectToAction("Index", "Cars");
        
        [HttpGet]
        public IActionResult CreateCar() => View(new CarDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCar(CarDto dto, string? imageUrlsString)
        {
            if (!ModelState.IsValid) return View(dto);
            
            if (!string.IsNullOrWhiteSpace(imageUrlsString))
                dto.ImageUrls = imageUrlsString
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToList();
            else
                dto.ImageUrls = new List<string>();

            try
            {
                await _api.CreateCar(dto);
                TempData["AlertMessage"] = "Annons skapad!";
                TempData["AlertType"] = "success";
                return RedirectToAction(nameof(ShowCars));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Kunde inte skapa bil: {ex.Message}");
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCar(int id)
        {
            try
            {
                var car = await _api.GetCarId(id);
                if (car is null) return NotFound();
                return View(car);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCar(CarDto dto, string? imageUrlsString)
        {
            if (!ModelState.IsValid) return View(dto);
            
            dto.ImageUrls = !string.IsNullOrWhiteSpace(imageUrlsString)
                ? imageUrlsString.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
                : new List<string>();
            
            try
            {
                await _api.UpdateCar(dto.Id, dto);
                TempData["AlertMessage"] = "Annons uppdaterad!";
                TempData["AlertType"] = "warning";
                return RedirectToAction(nameof(ShowCars));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Kunde inte uppdatera bil: {ex.Message}");
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCar(int id)
        {
            try
            {
                var car = await _api.GetCarId(id);
                if (car is null) return NotFound();
                return View(car);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCarConfirmed(int id)
        {
            try
            {
                await _api.DeleteCar(id);
                TempData["AlertMessage"] = "Annons borttagen!";
                TempData["AlertType"]    = "danger"; 
                return RedirectToAction("Index", "Cars");
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"]    = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
            catch (HttpRequestException ex)
            {
                var msg = ex.Message.Contains("bokningar", StringComparison.OrdinalIgnoreCase) ||
                          ex.Message.Contains("409")
                    ? "Kunde inte ta bort: bilen har framtida bekräftade bokningar."
                    : "Kunde inte ta bort annonsen (kan vara 404 eller annat fel).";

                TempData["AlertMessage"] = msg;
                TempData["AlertType"]    = "warning";
                return RedirectToAction("ShowCars", "Admin");
            }
        }

        //Customers
        [HttpGet]
        public async Task<IActionResult> ShowCustomers()
        {
            try
            {
                var customers = await _api.GetCustomers();
                return View(customers);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails(int id)
        {
            try
            {
                var c = await _api.GetCustomer(id);
                if (c is null) return NotFound();
                return View(c);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var c = await _api.GetCustomerForDelete(id);
                if (c is null) return NotFound();
                return View(c);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            try
            {
                var ok = await _api.DeleteCustomer(id);
                TempData["AlertMessage"] = ok ? "Kund borttagen!" : "Kunde inte ta bort kund.";
                TempData["AlertType"] = ok ? "danger" : "secondary";
                return RedirectToAction(nameof(ShowCustomers));
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        //Bookings
        [HttpGet]
        public async Task<IActionResult> ShowBookings()
        {
            try
            {
                var list = await _api.GetBookings();
                return View(list);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            try
            {
                var ok = await _api.ConfirmBooking(id);
                TempData["AlertMessage"] = ok ? "Bokning bekräftad!" : "Kunde inte bekräfta bokningen.";
                TempData["AlertType"] = ok ? "success" : "secondary";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }

            return RedirectToAction(nameof(ShowBookings));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            try
            {
                var ok = await _api.DeleteBooking(id);
                TempData["AlertMessage"] = ok ? "Bokning borttagen!" : "Kunde inte ta bort bokningen.";
                TempData["AlertType"] = ok ? "danger" : "secondary";
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
            catch (HttpRequestException ex)
            {
                TempData["AlertMessage"] = $"Fel vid borttagning: {ex.Message}";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction(nameof(ShowBookings));
        }
    }
}
