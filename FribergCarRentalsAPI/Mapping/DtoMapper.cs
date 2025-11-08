using FribergCarRentalsAPI.DTOs;
using FribergCarRentalsAPI.Models;

namespace FribergCarRentalsAPI.Mapping
{
    public static class DtoMapper
    {
        public static CarDto MapToCarDto(Car car)
        {
            return new CarDto
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Color = car.Color,
                PriceRate = car.PriceRate,
                ImageUrls = car.ImageUrls
            };
        }

        public static BookingDto MapToBookingDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                CarId = booking.CarId,
                CustomerId = booking.CustomerId,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                IsConfirmed = booking.IsConfirmed
            };
        }

        public static CustomerDto MapToCustomerDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email
            };
        }

        public static AdminDto MapToAdminDto(Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email
            };
        }
    }
}