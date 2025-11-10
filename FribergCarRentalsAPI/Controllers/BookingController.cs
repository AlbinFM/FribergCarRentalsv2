using System.Security.Claims;
using FribergCarRentalsAPI.Constants;
using FribergCarRentalsAPI.Data;
using FribergCarRentalsAPI.Data.Models;
using FribergCarRentalsAPI.DTOs;
using FribergCarRentalsAPI.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/booking/create?carId=123
        [HttpGet("create")]
        [Authorize(Roles = ApiRoles.User)]
        public async Task<IActionResult> CreateBookingAsync([FromQuery] int carId)
        {
            var carExists = await _context.Cars.AnyAsync(c => c.Id == carId);
            if (!carExists) return NotFound();

            return Ok();
        }

        // POST: api/booking/create
        // Skapar bokning för INLOGGAD kund (CustomerId tas alltid från token)
        [HttpPost("create")]
        [Authorize(Roles = ApiRoles.User)]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Grundvalidering av datum
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "Slutdatum måste vara efter startdatum." });

            // Säkerställ att bilen finns
            var carExists = await _context.Cars.AnyAsync(c => c.Id == dto.CarId);
            if (!carExists) return NotFound();

            // Hämtar kund via token
            var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(apiUserId))
                return Unauthorized();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);
            if (customer is null)
                return NotFound();

            // (Valfritt) kontrollera överlappande bokningar för bilen:
            var overlaps = await _context.Bookings.AnyAsync(b =>
                b.CarId == dto.CarId &&
                b.IsConfirmed &&
                b.EndDate >= dto.StartDate &&
                b.StartDate <= dto.EndDate);
            if (overlaps)
                return Conflict();

            var booking = new Booking
            {
                CarId = dto.CarId,
                CustomerId = customer.Id, 
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsConfirmed = false  
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(DtoMapper.MapToBookingDto(booking));
        }

        // GET: api/booking/confirmation
        [HttpGet("confirmation")]
        [Authorize(Roles = ApiRoles.User)]
        public IActionResult Confirmation()
        {
            return Ok();
        }

        // POST: api/booking/delete/{id}
        [HttpDelete("delete/{id:int}")]
        [Authorize] 
        public async Task<IActionResult> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            var isAdmin = User.IsInRole(ApiRoles.Admin);
            if (!isAdmin)
            {
                var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(apiUserId)) return Unauthorized();

                var callerCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);
                if (callerCustomer is null || booking.CustomerId != callerCustomer.Id)
                    return Forbid();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bokning avbokad!", customerId = booking.CustomerId });
        }

        // POST: api/booking/confirm/{id}
        [HttpPost("confirm/{id:int}")]
        [Authorize(Roles = ApiRoles.Admin)]
        public async Task<IActionResult> ConfirmBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            booking.IsConfirmed = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bokning bekräftad!" });
        }

        // GET: api/booking/my?customerId=123
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> MyBookingsAsync([FromQuery] int customerId)
        {
            var isAdmin = User.IsInRole(ApiRoles.Admin);
            if (!isAdmin)
            {
                var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(apiUserId)) return Unauthorized();

                var callerCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);
                if (callerCustomer is null || callerCustomer.Id != customerId)
                    return Forbid();
            }

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();

            return Ok(bookings.Select(DtoMapper.MapToBookingDto));
        }
    }
}
