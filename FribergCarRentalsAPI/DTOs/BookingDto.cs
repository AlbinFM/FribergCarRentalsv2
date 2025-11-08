using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        
        [Required]
        public int CarId { get; set; }
        
        [Required]
        public int CustomerId { get; set; }
        
        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
        
        public bool IsConfirmed { get; set; }
    }
}