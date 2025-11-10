using FribergCarRentalsAPI.Data;
using FribergCarRentalsAPI.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/cars
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bookedCarIds = await _context.Bookings
                .Where(b => b.IsConfirmed && b.EndDate >= DateTime.Today)
                .Select(b => b.CarId)
                .ToListAsync();

            var availableCars = await _context.Cars
                .Where(c => !bookedCarIds.Contains(c.Id))
                .ToListAsync();

            var dtoList = availableCars
                .Select(car => DtoMapper.MapToCarDto(car))
                .ToList();

            return Ok(dtoList);
        }

        // GET: api/cars/details/5
        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> CarDetailsAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id.Value);

            if (car == null)
                return NotFound();

            var dto = DtoMapper.MapToCarDto(car);

            return Ok(dto);
        }
    }
}