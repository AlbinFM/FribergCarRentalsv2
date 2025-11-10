using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergCarRentalsAPI.Data.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required] public string Brand { get; set; } = string.Empty;
        [Required] public string Model { get; set; } = string.Empty;
        [Range(1900,2100)] public int Year { get; set; }
        [Required] public string Color { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue)]
        public decimal PriceRate { get; set; }
        
        [Required]
        public string ImageUrlsCsv { get; set; } = string.Empty;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}