using FribergCarRentalsMVC.ApiClients.Interfaces;
using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients;

public class BookingApiClient : BaseApiClient, IBookingApiClient
{
    public BookingApiClient(HttpClient http) : base(http) { }

    public Task<BookingsAllDto.BookingDto> CreateBooking(
        BookingsAllDto.CreateBookingDto dto,
        CancellationToken ct = default)
        => PostAsync<BookingsAllDto.BookingDto>("api/booking/create", dto, ct)!;

    // API: [HttpDelete("delete/{id:int}")]
    public async Task<bool> DeleteBooking(int id, CancellationToken ct = default)
    {
        await DeleteAsync($"api/booking/delete/{id}", ct);
        return true;
    }

    public async Task<bool> ConfirmBooking(int id, CancellationToken ct = default)
    {
        await PostAsync<object>($"api/booking/confirm/{id}", null, ct);
        return true;
    }

    public Task<List<BookingsAllDto.BookingDto>> MyBookings(int customerId, CancellationToken ct = default)
        => GetAsync<List<BookingsAllDto.BookingDto>>($"api/booking/my?customerId={customerId}", ct)!;
}