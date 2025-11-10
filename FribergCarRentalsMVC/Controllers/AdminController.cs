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

        // ===== Dashboard/Index =====
        [HttpGet]
        public IActionResult Dashboard() => View();

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        // ===== Cars =====
        [HttpGet]
        public async Task<IActionResult> ShowCars()
        {
            try
            {
                var cars = await _api.GetCars();
                return View(cars);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

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

            try
            {
                await _api.UpdateCar(dto.Id, dto, imageUrlsString);

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
                var ok = await _api.DeleteCar(id);
                TempData["AlertMessage"] = ok ? "Annons borttagen!" : "Kunde inte ta bort.";
                TempData["AlertType"] = ok ? "danger" : "secondary";
                return RedirectToAction(nameof(ShowCars));
            }
            catch (UnauthorizedAccessException)
            {
                TempData["AlertMessage"] = "Din session har gått ut. Logga in igen.";
                TempData["AlertType"] = "warning";
                return RedirectToAction("AdminLogin", "AdminAuth");
            }
        }

        // ===== Customers =====
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

        [HttpPost, ActionName("DeleteCustomer")]
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

        // ===== Bookings =====
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
    }
}
