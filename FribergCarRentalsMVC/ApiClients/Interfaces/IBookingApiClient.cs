using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces;

public interface IBookingApiClient
{
    Task<BookingDto> CreateBooking(CreateBookingDto dto, CancellationToken ct = default);
    Task<bool> DeleteBooking(int id, CancellationToken ct = default);
    Task<List<BookingDto>> MyBookings( CancellationToken ct = default);
}