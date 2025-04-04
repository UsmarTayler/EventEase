using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Data
{
    public class EventEaseDbContext : DbContext
    {
        public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map the Booking entity to the "booking" table
            modelBuilder.Entity<Booking>().ToTable("booking");

            // Map the Venue entity to the "Venue" table
            modelBuilder.Entity<Venue>().ToTable("Venue");

            // Map the Event entity to the "Event" table
            modelBuilder.Entity<Event>().ToTable("Event");

            // Add any additional mappings or configurations here
        }
    }
}


