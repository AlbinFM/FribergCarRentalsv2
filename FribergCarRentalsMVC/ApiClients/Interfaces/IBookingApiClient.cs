using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces;

public interface IBookingApiClient
{
    Task<BookingsAllDto.BookingDto> CreateBooking(BookingsAllDto.CreateBookingDto dto, CancellationToken ct = default);
    Task<bool> DeleteBooking(int id, CancellationToken ct = default);
    Task<bool> ConfirmBooking(int id, CancellationToken ct = default);
    Task<List<BookingsAllDto.BookingDto>> MyBookings(int customerId, CancellationToken ct = default);
}