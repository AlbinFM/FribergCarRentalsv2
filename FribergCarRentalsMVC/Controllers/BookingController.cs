using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers;

public class BookingController : Controller
{
    private readonly IBookingApiClient _api;

    public BookingController(IBookingApiClient api)
    {
        _api = api;
    }

    // GET: Booking/CreateBooking
    [HttpGet]
    public IActionResult CreateBooking(int carId)
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId is null)
            return RedirectToAction("Login", "auth");
        
        var m = new CreateBookingDto
        {
            CarId = carId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today
        };

        return View(m);
    }

    // POST: Booking/CreateBooking
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        if (dto.StartDate > dto.EndDate)
            ModelState.AddModelError(nameof(dto.EndDate), "Slutdatum måste vara efter startdatum.");

        if (!ModelState.IsValid)
            return View(dto);

        try
        {
            await _api.CreateBooking(dto);

            TempData["Success"] = "Bokning skapad!";
            return RedirectToAction(nameof(Confirmation));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError("", $"Kunde inte skapa bokning: {ex.Message}");
            return View(dto);
        }
    }

    // GET: Booking/Confirmation
    [HttpGet]
    public IActionResult Confirmation() => View();
    
    // GET: /Booking/DeleteBooking
    [HttpGet]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var list = await _api.MyBookings();
        var booking = list.FirstOrDefault(b => b.Id == id);
        if (booking is null) return NotFound();

        return View(booking);
    }


    // POST: Booking/DeleteBooking/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBookingConfirmed(int id)
    {
        var ok = await _api.DeleteBooking(id);

        TempData["Success"] = ok ? "Bokningen är avbokad!" : "Kunde inte avboka bokningen.";

        return RedirectToAction(nameof(MyBookings));
    }

    // GET: Booking/MyBookings
    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId is null)
            return RedirectToAction("Login", "Auth");

        var list = await _api.MyBookings();

        return View(list);
    }
}
