using FribergCarRentalsAPI.Data.Models;
using FribergCarRentalsAPI.DTOs;

namespace FribergCarRentalsAPI.Mapping
{
    public static class DtoMapper
    {
        public static CarDto MapToCarDto(Car car) => new CarDto
        {
            Id = car.Id,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Color = car.Color,
            PriceRate = car.PriceRate,
            ImageUrls = string.IsNullOrWhiteSpace(car.ImageUrlsCsv)
                ? new List<string>()
                : car.ImageUrlsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };

        public static void MapToCarEntity(Car car, CarDto dto)
        {
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year = dto.Year;
            car.Color = dto.Color;
            car.PriceRate = dto.PriceRate;
            car.ImageUrlsCsv = dto.ImageUrls?.Any() == true
                ? string.Join(",", dto.ImageUrls)
                : string.Empty;
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
                IsConfirmed = booking.IsConfirmed,
                CarName = booking.Car != null ? $"{booking.Car.Brand} {booking.Car.Model}" : "Okänd bil",
                CustomerName = booking.Customer != null ? booking.Customer.FullName : "Okänd kund",
                Car = booking.Car == null ? null : MapToCarDto(booking.Car)
            };
        }
        

        public static CustomerDto MapToCustomerDto(Customer c)
        {
            return new CustomerDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email
            };
        }
    }
}