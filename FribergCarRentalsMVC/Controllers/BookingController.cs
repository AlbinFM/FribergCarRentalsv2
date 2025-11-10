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

    // GET: Booking/CreateBookingAsync?carId=5
    [HttpGet]
    public IActionResult CreateBooking(int carId)
    {
        // Hämta customerId från session
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId is null)
            return RedirectToAction("Login", "Customer");
        
        var m = new BookingsAllDto.CreateBookingDto
        {
            CarId = carId,
            CustomerId = customerId.Value,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today
        };

        return View(m);
    }

    // POST: Booking/CreateBookingAsync
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBooking(BookingsAllDto.CreateBookingDto dto)
    {
        // Date validation
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


    // POST: Booking/DeleteBookingAsync/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var ok = await _api.DeleteBooking(id);

        TempData["Success"] = ok ? "Bokningen är avbokad!" : "Kunde inte avboka bokningen.";

        return RedirectToAction(nameof(MyBookings));
    }

    // GET: Booking/MyBookingsAsync
    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");

        if (customerId is null)
            return RedirectToAction("Login", "Customer");

        var list = await _api.MyBookings(customerId.Value);

        return View(list);
    }
}
