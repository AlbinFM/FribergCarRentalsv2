using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }

        [Required] public string Brand { get; set; } = string.Empty;
        [Required] public string Model { get; set; } = string.Empty;

        [Range(1900, 2100)]
        public int Year { get; set; }

        [Required] public string Color { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Priset måste vara större än noll")]
        public decimal PriceRate { get; set; }
        
        public List<string> ImageUrls { get; set; } = new();
    }

}