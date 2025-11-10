using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FribergCarRentalsAPI.Constants;
using FribergCarRentalsAPI.Data;
using FribergCarRentalsAPI.Data.Models;
using FribergCarRentalsAPI.DTOs;
using FribergCarRentalsAPI.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentalsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public CustomerController(
            AppDbContext context,
            UserManager<ApiUser> userManager,
            IConfiguration config,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
            _roleManager = roleManager;
        }


        // GET: api/customer/register
        [HttpGet("register")]
        public IActionResult CustomerRegister()
        {
            return Ok();
        }

        // POST: api/customer/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerRegister([FromBody] CustomerDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Kolla om email används redan
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict(new { message = "Email används redan." });

            // Skapa Identity user
            var apiUser = new ApiUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FullName.Split(' ').FirstOrDefault(),
                LastName  = dto.FullName.Split(' ').Skip(1).FirstOrDefault() ?? ""
            };

            var result = await _userManager.CreateAsync(apiUser, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Se till att Customer-rollen finns
            if (!await _roleManager.RoleExistsAsync(ApiRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(ApiRoles.User));

            await _userManager.AddToRoleAsync(apiUser, ApiRoles.User);

            // Skapa Customer-profil
            var customer = new Customer
            {
                FullName  = dto.FullName,
                Email     = dto.Email,
                ApiUserId = apiUser.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return Ok(DtoMapper.MapToCustomerDto(customer));
        }
        
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok();
        }
        
        
        [HttpGet("bookings")]
        [Authorize(Roles = ApiRoles.User)]
        public async Task<IActionResult> MyBookingsAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApiUserId == userId);
            if (customer == null) return NotFound();

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerId == customer.Id)
                .ToListAsync();

            return Ok(bookings.Select(DtoMapper.MapToBookingDto).ToList());
        }
        
        // API: CustomerController
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var apiUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(apiUserId)) return Unauthorized();

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ApiUserId == apiUserId);

            if (customer is null) return NotFound();

            var dto = DtoMapper.MapToCustomerDto(customer);
            return Ok(dto);
        }
        
    }
}
