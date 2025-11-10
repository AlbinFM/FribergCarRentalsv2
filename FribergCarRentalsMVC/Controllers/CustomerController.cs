using FribergCarRentalsMVC.DTOs;
using FribergCarRentalsMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IAuthService _auth;

        public CustomerController(IAuthService auth) => _auth = auth;

        // GET: /Customer/Register
        [HttpGet]
        public IActionResult Register() => View(new CustomersAllDto.CustomerRegisterDto());

        // POST: /Customer/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(CustomersAllDto.CustomerRegisterDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                // Försök registrera och logga in direkt
                var ok = await _auth.RegisterAndLogin(dto);
                if (!ok)
                {
                    TempData["AlertMessage"] = "Konto skapat, men autoinloggning misslyckades. Logga in nedan.";
                    TempData["AlertType"] = "warning";
                    return RedirectToAction(nameof(Login));
                }

                TempData["AlertMessage"] = "Välkommen! Du är nu inloggad.";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already in use", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(dto.Email), "E-postadressen används redan.");
                return View(dto);
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Registrering misslyckades: {ex.Message}");
                return View(dto);
            }
        }

        // GET: /Customer/Login
        [HttpGet]
        public IActionResult Login() => View(new CustomersAllDto.LoginDto());

        // POST: /Customer/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CustomersAllDto.LoginDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var ok = await _auth.Login(dto, fetchProfile: true);
                if (!ok)
                {
                    ModelState.AddModelError("", "Fel e-post eller lösenord.");
                    return View(dto);
                }

                TempData["AlertMessage"] = "Inloggad!";
                TempData["AlertType"] = "success";
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Inloggning misslyckades: {ex.Message}");
                return View(dto);
            }
        }

        // POST: /Customer/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _auth.Logout();
            TempData["AlertMessage"] = "Du har loggats ut.";
            TempData["AlertType"] = "info";
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
