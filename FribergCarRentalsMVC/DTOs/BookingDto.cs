
namespace FribergCarRentalsMVC.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        
        public int CarId { get; set; }
        
        public int CustomerId { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public bool IsConfirmed { get; set; }
        
        public CarDto? Car { get; set; }

        public string CarName { get; set; } = string.Empty;
        
        public string CustomerName { get; set; } = string.Empty;

    }
}