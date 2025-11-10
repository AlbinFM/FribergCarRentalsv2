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
        
        // GET: api/admin
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index() =>
            Ok(new { message = "Use POST /api/auth/login for admin sign-in." });

        // GET: api/admin/adminlogin
        [HttpGet("adminlogin")]
        [AllowAnonymous]
        public IActionResult AdminLogin() =>
            Ok(new { message = "Login via POST /api/auth/login with admin credentials." });

        // POST: api/admin/login (behåll namnet men ge hint)
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult LoginAsync([FromQuery] string? email, [FromQuery] string? password) =>
            BadRequest(new { message = "Use POST /api/auth/login with JSON body { email, password }." });

        // POST: api/admin/logout  (JWT logout är klient-sides)
        [HttpPost("logout")]
        public IActionResult Logout() =>
            Ok(new { message = "Discard the JWT on the client to logout." });

        // GET: api/admin/dashboard
        [HttpGet("dashboard")]
        public IActionResult Dashboard() =>
            Ok(new { message = "Admin dashboard placeholder (API)." });

        // ======= CARS =======

        // GET: api/admin/cars
        [HttpGet("cars")]
        public async Task<IActionResult> ShowCarsAsync()
        {
            var cars = await _context.Cars.AsNoTracking().ToListAsync();
            var dto = cars.Select(DtoMapper.MapToCarDto);
            return Ok(dto);
        }

        // GET: api/admin/cars/create (ersätter formulär—ger instruktion)
        [HttpGet("cars/create")]
        public IActionResult CreateCarAsync() =>
            Ok(new
            {
                message = "Create car by POSTing JSON to /api/admin/cars",
                example = new { brand = "Volvo", model = "V60", year = 2021, color = "Black", priceRate = 899.00m, imageUrls = new[] { "https://..." } }
            });

        [HttpPost("cars")]
        public async Task<IActionResult> CreateCarAsync([FromBody] CarDto carDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var car = new Car();
            DtoMapper.MapToCarEntity(carDto, car);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(EditCarAsync), new { id = car.Id }, DtoMapper.MapToCarDto(car));
        }

        // GET: api/admin/cars/{id}
        [HttpGet("cars/{id:int}")]
        public async Task<IActionResult> EditCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            return Ok(DtoMapper.MapToCarDto(car));
        }

        [HttpPost("cars/{id:int}")]
        public async Task<IActionResult> EditCarAsync(int id, [FromBody] CarDto carDto, [FromQuery] string? imageUrlsString)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            DtoMapper.MapToCarEntity(carDto, car);

            if (!string.IsNullOrWhiteSpace(imageUrlsString))
                car.ImageUrlsCsv = imageUrlsString;

            await _context.SaveChangesAsync();
            return Ok(DtoMapper.MapToCarDto(car));
        }

        // GET: api/admin/cars/delete/{id}
        [HttpGet("cars/delete/{id:int}")]
        public async Task<IActionResult> DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            return Ok(DtoMapper.MapToCarDto(car));
        }

        // POST: api/admin/cars/delete/{id}
        [HttpPost("cars/delete/{id:int}")]
        public async Task<IActionResult> DeleteCarConfirmedAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            var hasFutureConfirmed = await _context.Bookings.AnyAsync(b =>
                b.CarId == id &&
                b.IsConfirmed &&
                b.EndDate >= DateTime.Today);

            if (hasFutureConfirmed)
                return Conflict();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        

        // GET: api/admin/customers
        [HttpGet("customers")]
        public async Task<IActionResult> ShowCustomersAsync()
        {
            var customers = await _context.Customers.AsNoTracking().ToListAsync();
            var dto = customers.Select(DtoMapper.MapToCustomerDto);
            return Ok(dto);
        }

        // GET: api/admin/customers/{id}
        [HttpGet("customers/{id:int}")]
        public async Task<IActionResult> CustomerDetailsAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .ThenInclude(b => b.Car)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            return Ok(DtoMapper.MapToCustomerDto(customer)); 
        }

        // GET: api/admin/customers/delete/{id}
        [HttpGet("customers/delete/{id:int}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            return Ok(DtoMapper.MapToCustomerDto(customer));
        }

        // POST: api/admin/customers/delete/{id}
        [HttpPost("customers/delete/{id:int}")]
        public async Task<IActionResult> DeleteCustomerConfirmedAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return NotFound();

            if (customer.Bookings.Any())
                _context.Bookings.RemoveRange(customer.Bookings);

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/admin/bookings
        [HttpGet("bookings")]
        public async Task<IActionResult> ShowBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .AsNoTracking()
                .ToListAsync();

            var dto = bookings.Select(DtoMapper.MapToBookingDto);
            return Ok(dto);
        }
    }
}
