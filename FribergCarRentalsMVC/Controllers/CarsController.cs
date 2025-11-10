using FribergCarRentalsMVC.ApiClients.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FribergCarRentalsMVC.Controllers
{
    public class CarsController : Controller
    {
        private readonly ICarsApiClient _api;

        public CarsController(ICarsApiClient api)
        {
            _api = api;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _api.GetAvailableCars();
            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> CarDetails(int? id)
        {
            if (id is null) return NotFound();
            var car = await _api.GetDetails(id.Value);
            if (car is null) return NotFound();
            return View(car);
        }
    }
}