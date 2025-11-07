using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsMVC.Models
{
    public class Customer
    {
        public int Id { get; set; } // Id för kunden

        [Required]
        public string FullName { get; set; } = string.Empty; // Fullständigt namn på kunden

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty; // E-postadress för kunden

        [Required]
        public string Password { get; set; } = string.Empty; // Lösenord för kunden

        public ICollection<Booking> Bookings { get; set; } // Lista med bokningar som kunden har gjort

        public Customer() // Konstruktor som initialiserar listan med bokningar
        {
            Bookings = new List<Booking>();
        }
    }
}