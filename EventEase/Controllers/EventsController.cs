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
    // Debug: Log all submitted values for troubleshooting
    Console.WriteLine($"EventName: {@event.EventName}, EventDate: {@event.EventDate}, Description: {@event.Description}, VenueId: {@event.VenueId}");

    // Step 1: Validate the ModelState
    if (ModelState.IsValid)
    {
        try
        {
            // Step 2: Add the event to the database and log debug information
            _context.Add(@event);
            Console.WriteLine($"Saving Event: {@event.EventName}, VenueId: {@event.VenueId}");
            await _context.SaveChangesAsync();
            Console.WriteLine("Event saved successfully.");
            return RedirectToAction(nameof(Index)); // Redirect to Index on success
        }
        catch (Exception ex)
        {
            // Step 3: Handle database-related exceptions
            Console.WriteLine($"Database Save Error: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while saving the event. Please try again.");
        }
    }
    else
    {
        // Step 4: Log validation errors for debugging purposes
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        }
    }

    // Step 5: Ensure dropdown retains selected value on failure
    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", @event.VenueId); // Using descriptive names (VenueName)
    return View(@event); // Return the view with validation and error messages
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
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}

