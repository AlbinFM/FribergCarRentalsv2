using System.Security.Claims;
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

        public CustomerController(
            AppDbContext context,
            UserManager<ApiUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // POST: api/customer/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> CustomerRegister([FromBody] CustomerRegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return Conflict(new { message = "Email används redan." });

            var apiUser = new ApiUser
            {
                UserName  = dto.Email,
                Email     = dto.Email,
                FirstName = dto.FullName.Split(' ').FirstOrDefault(),
                LastName  = dto.FullName.Split(' ').Skip(1).FirstOrDefault() ?? ""
            };

            var createResult = await _userManager.CreateAsync(apiUser, dto.Password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);

            if (!await _roleManager.RoleExistsAsync(ApiRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(ApiRoles.User));
            await _userManager.AddToRoleAsync(apiUser, ApiRoles.User);

            var customer = new Customer
            {
                FullName  = dto.FullName,
                Email     = dto.Email,
                ApiUserId = apiUser.Id
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var outDto = DtoMapper.MapToCustomerDto(customer);
            return CreatedAtAction(nameof(GetProfile), new { }, outDto);
        }

        // POST: api/customer/logout 
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout() => Ok();

        // GET: api/customer/bookings
        [HttpGet("bookings")]
        [Authorize(Roles = ApiRoles.User)]
        public async Task<IActionResult> MyBookings()
        {
            var me = await GetCurrentCustomer();
            if (me is null) return Unauthorized();

            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Where(b => b.CustomerId == me.Id)
                .AsNoTracking()
                .ToListAsync();

            return Ok(bookings.Select(DtoMapper.MapToBookingDto).ToList());
        }

        // GET: api/customer/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var me = await GetCurrentCustomer(track: false);
            if (me is null) return Unauthorized();

            return Ok(DtoMapper.MapToCustomerDto(me));
        }
        
        private async Task<Customer?> GetCurrentCustomer(bool track = true)
        {
            var apiUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(apiUserId)) return null;

            var query = _context.Customers.Where(c => c.ApiUserId == apiUserId);
            if (!track) query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }
    }
}
