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
        public BookingController(AppDbContext context) => _context = context;

        // POST: api/booking/create
        [HttpPost("create")]
        [Authorize(Roles = ApiRoles.User)]
        public async Task<IActionResult> CreateBookingAsync([FromBody] CreateBookingDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "Slutdatum måste vara efter startdatum." });
            
            var carExists = await _context.Cars.AnyAsync(c => c.Id == dto.CarId);
            if (!carExists) return NotFound(new { message = "Bilen finns inte." });
            
            var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(apiUserId)) return Unauthorized();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);
            if (customer is null) return NotFound(new { message = "Kundprofil saknas." });
            
            var overlaps = await _context.Bookings.AnyAsync(b =>
                b.CarId == dto.CarId &&
                b.IsConfirmed &&
                b.EndDate >= dto.StartDate &&
                b.StartDate <= dto.EndDate);
            if (overlaps) return Conflict(new { message = "Bilen är redan bokad dessa datum." });

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

        // GET: api/booking/{id}
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car).Include(b => b.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking is null) return NotFound();
            return Ok(DtoMapper.MapToBookingDto(booking));
        }

        // DELETE: api/booking/{id}
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            var isAdmin = User.IsInRole(ApiRoles.Admin);
            if (!isAdmin)
            {
                var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(apiUserId)) return Unauthorized();

                var me = await _context.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);
                if (me is null || me.Id != booking.CustomerId) return Forbid();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
