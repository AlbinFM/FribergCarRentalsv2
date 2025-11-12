using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients;

public class BookingApiClient : BaseApiClient, IBookingApiClient
{
    public BookingApiClient(HttpClient http) : base(http) { }

    public Task<BookingDto> CreateBooking(CreateBookingDto dto, CancellationToken ct = default)
        => PostAsync<BookingDto>("api/booking/create", dto, ct)!;

    public async Task<bool> DeleteBooking(int id, CancellationToken ct = default)
    {
        await DeleteAsync($"api/booking/{id}", ct);
        return true;
    }
    
    public Task<List<BookingDto>> MyBookings(CancellationToken ct = default)
        => GetAsync<List<BookingDto>>("api/customer/bookings", ct)!;
}