using FribergCarRentalsMVC.DTOs;

namespace FribergCarRentalsMVC.ApiClients.Interfaces
{
    public interface IAdminApiClient
    {
        // Cars
        Task<CarDto?> GetCarId(int id, CancellationToken ct = default);
        Task<CarDto> CreateCar(CarDto dto, CancellationToken ct = default);
        public Task<CarDto> UpdateCar(int id, CarDto dto, CancellationToken ct = default);
        Task<bool> DeleteCar(int id, CancellationToken ct = default);

        // Customers
        Task<List<CustomersAllDto.CustomerDto>> GetCustomers(CancellationToken ct = default);
        Task<CustomersAllDto.CustomerDto?> GetCustomer(int id, CancellationToken ct = default);
        Task<CustomersAllDto.CustomerDto?> GetCustomerForDelete(int id, CancellationToken ct = default);
        Task<bool> DeleteCustomer(int id, CancellationToken ct = default);

        // Bookings
        Task<List<BookingDto>> GetBookings(CancellationToken ct = default);
        Task<bool> ConfirmBooking(int id, CancellationToken ct = default);
        Task<bool> DeleteBooking(int id, CancellationToken ct = default);

    }
}