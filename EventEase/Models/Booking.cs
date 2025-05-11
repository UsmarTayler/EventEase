using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EventEase.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Event selection is required")]
        public int EventId { get; set; }

        [ValidateNever]
        public Event Event { get; set; }

        [Required(ErrorMessage = "Venue selection is required")]
        public int VenueId { get; set; }

        [ValidateNever]
        public Venue Venue { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }
}

