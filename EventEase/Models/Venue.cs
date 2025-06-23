using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEase.Models
{
    public class Venue
    {
        [Required(ErrorMessage = "Please select a venue.")]
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
        public bool IsAvailable { get; set; }

        public int? EventTypeId { get; set; }

        [ForeignKey("EventTypeId")]
        public EventType EventType { get; set; }




    }
}
