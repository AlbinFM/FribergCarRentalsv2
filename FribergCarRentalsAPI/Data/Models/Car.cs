using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergCarRentalsAPI.Models
{
    public class Car
    {
        public int Id { get; set; } // Id för bilen

        [Required]
        public string Brand { get; set; } = string.Empty; // Märke på bilen

        [Required]
        public string Model { get; set; } = string.Empty; // Modell på bilen

        [Range(1900, 2100)]
        public int Year { get; set; } // Tillverkningsår för bilen

        [Required]
        public string Color { get; set; } = string.Empty; // Färg på bilen

        [Required]
        [Display(Name = "Pris per dag")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Priset måste vara större än noll")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceRate { get; set; } // Pris per dag för att hyra bilen

        [Required]
        public List<string> ImageUrls { get; set; } // Lista med URL:er till bilens bilder

        public ICollection<Booking> Bookings { get; set; } // Lista med bokningar för bilen

        public Car() // Konstruktor som initialiserar listorna
        {
            ImageUrls = new List<string>();
            Bookings = new List<Booking>();
        }
    }
}