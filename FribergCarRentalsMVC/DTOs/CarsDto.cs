using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FribergCarRentalsMVC.DTOs
{
    public class CarDto
    {
        public int Id { get; set; }
        
        [Required]
        public string Brand { get; set; }
        
        [Required]
        public string Model { get; set; }
        
        [Range(1900, 2100)]
        public int Year { get; set; }
        
        [Required]
        public string Color { get; set; }
        
        [Required]
        [Display(Name = "Pris per dag")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Priset måste vara större än noll")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceRate { get; set; }
        
        [Required]
        public List<string> ImageUrls { get; set; } = new();

    }
}