using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsMVC.DTOs
{
    public static class BookingsAllDto
    {
        public class BookingDto
        {
            public int Id { get; set; }
            public int CarId { get; set; }
            public int CustomerId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsConfirmed { get; set; }
        }

        public class CreateBookingDto
        {
            [Required] public int CarId { get; set; }
            
            [Required] public int CustomerId { get; set; }

            [Required, DataType(DataType.Date)]
            public DateTime StartDate { get; set; }

            [Required, DataType(DataType.Date)]
            public DateTime EndDate { get; set; }
        }
        
    }
}