using Microsoft.EntityFrameworkCore;
using EventEase.Models;
using System.Collections.Generic;

namespace EventEase.Data
{
    public class EventEaseDbContext : DbContext
    {
        public EventEaseDbContext(DbContextOptions<EventEaseDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }
    }
}
