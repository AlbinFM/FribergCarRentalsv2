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
            ImageUrls = (car.ImageUrlsCsv ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList()
        };

        public static void MapToCarEntity(CarDto dto, Car car)
        {
            car.Brand = dto.Brand;
            car.Model = dto.Model;
            car.Year  = dto.Year;
            car.Color = dto.Color;
            car.PriceRate = dto.PriceRate;
            car.ImageUrlsCsv = string.Join(",", dto.ImageUrls ?? new List<string>());
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

        public static CustomerDto MapToCustomerDto(Customer customerDto)
        {
            return new CustomerDto
            {
                Id = customerDto.Id,
                FullName = customerDto.FullName,
                Email = customerDto.Email
            };
        }
    }
}