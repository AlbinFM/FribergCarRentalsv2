using FribergCarRentalsMVC.DTOs;
using FribergCarRentalsMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers;

public class AdminAuthController : Controller
{
    private readonly IAuthService _auth;
    public AdminAuthController(IAuthService auth) => _auth = auth;

    [HttpGet]
    public IActionResult AdminLogin() => View(new AdminLoginDto());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(AdminLoginDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var ok = await _auth.AdminLogin(dto);
        if (!ok || !_auth.IsAdmin())
        {
            await _auth.Logout();
            ModelState.AddModelError("", "Fel e-post eller lösenord. Försök igen.");
            return View(dto);
        }

        TempData["AlertMessage"] = "Välkommen admin!";
        TempData["AlertType"] = "success";
        return RedirectToAction("Dashboard", "Admin");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogout()
    {
        await _auth.Logout();
        return RedirectToAction(nameof(Index), "Home", new { area = ""});
    }
}