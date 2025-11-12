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
    [Authorize(Roles = ApiRoles.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context) => _context = context;

        // Cars

        // GET: api/admin/cars
        [HttpGet("cars")]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _context.Cars.AsNoTracking().ToListAsync();
            var dto = cars.Select(DtoMapper.MapToCarDto);
            return Ok(dto);
        }

        // GET: api/admin/cars/5
        [HttpGet("cars/{id:int}", Name = "GetAdminCarById")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _context.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (car is null) return NotFound();
            return Ok(DtoMapper.MapToCarDto(car));
        }

        // POST: api/admin/cars
        [HttpPost("cars")]
        public async Task<IActionResult> CreateCar([FromBody] CarDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var car = new Car();
            DtoMapper.MapToCarEntity(car, dto);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetAdminCarById", new { id = car.Id }, DtoMapper.MapToCarDto(car));
        }

        // PUT: api/admin/cars/5
        [HttpPut("cars/{id:int}")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] CarDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var car = await _context.Cars.FindAsync(id);
            if (car is null) return NotFound();

            DtoMapper.MapToCarEntity(car, dto);
            await _context.SaveChangesAsync();

            return Ok(DtoMapper.MapToCarDto(car));
        }

        // DELETE: api/admin/cars/{id}
        [HttpDelete("cars/{id:int}")]
        public async Task<IActionResult> DeleteCarConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            var hasFutureConfirmed = await _context.Bookings.AnyAsync(b =>
                b.CarId == id && b.IsConfirmed && b.EndDate >= DateTime.Today);

            if (hasFutureConfirmed)
                return Conflict(new { message = "Bilen är redan bokad då." });

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        // Customers

        // GET: api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _context.Customers.AsNoTracking().ToListAsync();
            var dto = customers.Select(DtoMapper.MapToCustomerDto);
            return Ok(dto);
        }

        // GET: api/admin/customers/5
        [HttpGet("customers/{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Car)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return NotFound();
            return Ok(DtoMapper.MapToCustomerDto(customer));
        }

        // DELETE: api/admin/customers/5
        [HttpDelete("customers/{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null) return NotFound();

            if (customer.Bookings != null && customer.Bookings.Any())
                _context.Bookings.RemoveRange(customer.Bookings);

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Bookings

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        [Authorize(Roles = ApiRoles.Admin)]
        public async Task<IActionResult> GetAllBookings()
        {
            var list = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .AsNoTracking()
                .ToListAsync();

            return Ok(list.Select(DtoMapper.MapToBookingDto).ToList());
        }
        
        // POST: api/admin/{id}/confirm
        [HttpPost("{id:int}/confirm")]
        [Authorize(Roles = ApiRoles.Admin)]
        public async Task<IActionResult> ConfirmBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking is null) return NotFound();

            booking.IsConfirmed = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bokning bekräftad." });
        }
    }
}
