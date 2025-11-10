using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.Data.Models
{
    public class Customer
    {
        public int Id { get; set; } // Id för kunden

        [Required]
        public string FullName { get; set; } = string.Empty; // Fullständigt namn på kunden

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // E-postadress för kunden

        //Koppling till identity
        public string ApiUserId { get; set; } = string.Empty;

        public ApiUser ApiUser { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } // Lista med bokningar som kunden har gjort

        public Customer() // Konstruktor som initialiserar listan med bokningar
        {
            Bookings = new List<Booking>();
        }
    }
}