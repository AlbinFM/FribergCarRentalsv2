using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients;

public class AdminApiClient : BaseApiClient, IAdminApiClient
{
    public AdminApiClient(HttpClient http) : base(http) { }

    // Cars
    public Task<List<CarDto>> GetCars(CancellationToken ct = default)
        => GetAsync<List<CarDto>>("api/admin/cars", ct)!;

    public Task<CarDto?> GetCarId(int id, CancellationToken ct = default)
        => GetAsync<CarDto>($"api/admin/cars/{id}", ct);

    public Task<CarDto> CreateCar(CarDto dto, CancellationToken ct = default)
        => PostAsync<CarDto>("api/admin/cars", dto, ct)!;

    public Task UpdateCar(int id, CarDto dto, CancellationToken ct = default)
        => PutAsync($"api/admin/cars/{id}", dto, ct);

    public async Task<bool> DeleteCar(int id, CancellationToken ct = default)
    {
        await DeleteAsync($"api/admin/cars/{id}", ct);
        return true;
    }


    // Customers
    public Task<List<CustomersAllDto.CustomerDto>> GetCustomers(CancellationToken ct = default)
        => GetAsync<List<CustomersAllDto.CustomerDto>>("api/admin/customers", ct)!;

    public Task<CustomersAllDto.CustomerDto?> GetCustomer(int id, CancellationToken ct = default)
        => GetAsync<CustomersAllDto.CustomerDto>($"api/admin/customers/{id}", ct);

    public Task<CustomersAllDto.CustomerDto?> GetCustomerForDelete(int id, CancellationToken ct = default)
        => GetAsync<CustomersAllDto.CustomerDto>($"api/admin/customers/delete/{id}", ct);

    public async Task<bool> DeleteCustomer(int id, CancellationToken ct = default)
    {
        await DeleteAsync($"api/admin/customers/{id}", ct);
        return true;
    }

    // Bookings
    public Task<List<BookingDto>> GetBookings(CancellationToken ct = default)
        => GetAsync<List<BookingDto>>("api/admin/bookings", ct)!;
    
    public async Task<bool> ConfirmBooking(int id, CancellationToken ct = default)
    {
        await PostAsync<object>($"api/admin/{id}/confirm", null, ct);
        return true;
    }
    
    public async Task<bool> DeleteBooking(int id, CancellationToken ct = default)
    {
        await DeleteAsync($"api/booking/{id}", ct);
        return true;
    }
}
