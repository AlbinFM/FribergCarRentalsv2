using Microsoft.AspNetCore.WebUtilities;
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

    public Task<CarDto> UpdateCar(int id, CarDto dto, string? imageUrlsCsv = null, CancellationToken ct = default)
    {
        var url = $"api/admin/cars/{id}";
        if (!string.IsNullOrWhiteSpace(imageUrlsCsv))
        {
            url = QueryHelpers.AddQueryString(url, "imageUrlsString", imageUrlsCsv);
        }
        return PostAsync<CarDto>(url, dto, ct)!;
    }

    public async Task<bool> DeleteCar(int id, CancellationToken ct = default)
    {
        // API använder POST /api/admin/cars/delete/{id}
        await PostAsync<object>($"api/admin/cars/delete/{id}", null, ct);
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
        // API använder POST /api/admin/customers/delete/{id}
        await PostAsync<object>($"api/admin/customers/delete/{id}", null, ct);
        return true;
    }

    // Bookings
    public Task<List<BookingsAllDto.BookingDto>> GetBookings(CancellationToken ct = default)
        => GetAsync<List<BookingsAllDto.BookingDto>>("api/admin/bookings", ct)!;
}
