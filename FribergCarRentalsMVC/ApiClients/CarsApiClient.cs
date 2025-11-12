using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients
{
    public class CarsApiClient : BaseApiClient, ICarsApiClient
    {
        public CarsApiClient(HttpClient http) : base(http) {}

        public Task<List<CarDto>> GetAvailableCars(CancellationToken ct = default)
            => GetAsync<List<CarDto>>("api/cars", ct)!;

        public Task<CarDto?> GetDetails(int id, CancellationToken ct = default)
            => GetAsync<CarDto>($"api/cars/{id}", ct); 
    }
}