using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int CarId { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
    }
}