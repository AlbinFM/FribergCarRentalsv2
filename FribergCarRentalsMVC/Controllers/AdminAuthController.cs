using FribergCarRentalsMVC.DTOs;
using FribergCarRentalsMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly IAuthService _auth;

        public AdminAuthController(IAuthService auth) => _auth = auth;

        [HttpGet]
        public IActionResult AdminLogin() => View(new CustomersAllDto.LoginDto());

// AdminAuthController.cs (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogin(CustomersAllDto.LoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var ok = await _auth.LoginAdmin(dto);
                if (!ok)
                {
                    ModelState.AddModelError("", "Fel e-post eller lösenord.");
                    return View(dto);
                }

                if (!_auth.IsAdmin())
                {
                    await _auth.Logout();
                    ModelState.AddModelError("", "Du har inte administratörsbehörighet.");
                    return View(dto);
                }

                TempData["AlertMessage"] = "Välkommen, admin!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Dashboard", "Admin");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", $"Inloggning misslyckades: {ex.Message}");
                return View(dto);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminLogout()
        {
            await _auth.Logout();
            return RedirectToAction(nameof(AdminLogin));
        }
    }
}