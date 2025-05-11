using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required")]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a venue")]
        public int VenueId { get; set; }


        [ForeignKey("VenueId")]
        [ValidateNever] // <-- ADD THIS LINE
        public Venue Venue { get; set; }

    }
}
