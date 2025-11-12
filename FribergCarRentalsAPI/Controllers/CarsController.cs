using FribergCarRentalsAPI.Data;
using FribergCarRentalsAPI.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Controllers
{
    [ApiController]
    [Route("api/cars")] 
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CarsController(AppDbContext context) => _context = context;

        // GET: /api/cars
        [HttpGet]
        public async Task<IActionResult> GetAvailableCars()
        {
            var today = DateTime.UtcNow.Date;

            var availableCars = await _context.Cars
                .AsNoTracking()
                .Where(c => !_context.Bookings.Any(b =>
                    b.CarId == c.Id &&
                    b.IsConfirmed &&
                    b.EndDate >= today))
                .ToListAsync();

            var dtoList = availableCars.Select(DtoMapper.MapToCarDto).ToList();
            return Ok(dtoList);
        }

        // GET: /api/cars/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _context.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car is null) return NotFound();

            return Ok(DtoMapper.MapToCarDto(car));
        }
    }
}