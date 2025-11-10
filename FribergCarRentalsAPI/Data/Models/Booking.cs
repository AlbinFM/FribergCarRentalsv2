using System.ComponentModel.DataAnnotations;

namespace FribergCarRentalsAPI.Data.Models
{
    public class Booking
    {
        public int Id { get; set; } // Unikt ID för varje bokning

        [Required]
        public int CarId { get; set; } // Id till den bil som bokas

        public Car? Car { get; set; } // Bilen som bokas i objektform så att den kan tilldelas egenskaper

        [Required]
        public int CustomerId { get; set; } // Id till den kund som gör bokningen

        public Customer? Customer { get; set; } // Som ovan, kund i objektform

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; } // Startdatum för bokningen

        [Required]
        [Display(Name = "Slutdatum")]
        [DateRange]
        public DateTime EndDate { get; set; } // Slutdatum för bokningen

        public bool IsConfirmed { get; set; } = false; // Om bokningen är bekräftad eller inte
    }

    // Valideringsattribut jag hittade på nätet för att säkerställa att slutdatum är efter startdatum.
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var booking = (Booking)validationContext.ObjectInstance;

            if (booking.StartDate > booking.EndDate)
            {
                return new ValidationResult("Slutdatum måste vara efter startdatum.");
            }

            return ValidationResult.Success!;
        }
    }
}