using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventEaseDbContext _context;

        public EventsController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var eventEaseDbContext = _context.Events.Include(e => e.Venue);
            return View(await eventEaseDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            // Fetch venues from the database and populate the dropdown
            var venues = _context.Venues.ToList();
            foreach (var venue in venues)
            {
                Console.WriteLine($"VenueId: {venue.VenueId}, VenueName: {venue.VenueName}");
            }
            ViewData["VenueId"] = new SelectList(venues, "VenueId", "VenueName"); // Dropdown setup
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            Console.WriteLine("== SUBMITTED FORM DATA ==");
            Console.WriteLine($"EventName: {@event.EventName}");
            Console.WriteLine($"EventDate: {@event.EventDate}");
            Console.WriteLine($"Description: {@event.Description}");
            Console.WriteLine($"VenueId: {@event.VenueId}");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(@event);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Event saved successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Database Save Error: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the event. Please try again.");

                    // 🛠 FIX: Repopulate dropdown even if SaveChanges fails
                    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
                }
            }
            else
            {
                Console.WriteLine("❌ ModelState is invalid. Errors:");
                foreach (var entry in ModelState)
                {
                    string key = entry.Key;
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Field: {key} — Error: {error.ErrorMessage}");
                    }
                }

                // 🛠 FIX: Repopulate dropdown on model validation error
                ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId);
            }

            return View(@event);
        }




        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Events.Any(e => e.EventId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
                return NotFound();

            // ❌ Check for existing bookings for this event
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete this event because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }


        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}

