using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public required string Description { get; set; }
        
        public int? VenueId { get; set; } // Nullable foreign key
        [ForeignKey("VenueId")]
        public Venue Venue { get; set; } // Leave this without validation attributes




    }
}

