using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseDbContext _context;

        public BookingsController(EventEaseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            var bookingsQuery = _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                bookingsQuery = bookingsQuery.Where(b =>
                    b.BookingId.ToString().Contains(searchTerm) ||
                    b.Event.EventName.Contains(searchTerm));
            }

            var bookings = await bookingsQuery.ToListAsync();
            ViewData["CurrentFilter"] = searchTerm;
            return View(bookings);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(
                _context.Events.Include(e => e.Venue)
                    .Select(e => new
                    {
                        e.EventId,
                        Label = e.EventName + " — " +
                                e.EventDate.ToString("dd MMM yyyy HH:mm") + " — " +
                                e.Venue.VenueName
                    }).ToList(),
                "EventId", "Label"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,EventId,BookingDate")] Booking booking)
        {
            var selectedEvent = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event does not exist.");
            }
            else
            {
                booking.VenueId = selectedEvent.VenueId;

                bool isDoubleBooked = await _context.Bookings
                    .AnyAsync(b =>
                        b.VenueId == booking.VenueId &&
                        b.BookingDate == booking.BookingDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked at the selected date and time.");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(
                _context.Events.Include(e => e.Venue)
                    .Select(e => new
                    {
                        e.EventId,
                        Label = e.EventName + " — " +
                                e.EventDate.ToString("dd MMM yyyy HH:mm") + " — " +
                                e.Venue.VenueName
                    }).ToList(),
                "EventId", "Label", booking?.EventId
            );

            return View(booking);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewData["EventId"] = new SelectList(
                _context.Events.Include(e => e.Venue)
                    .Select(e => new
                    {
                        e.EventId,
                        Label = e.EventName + " — " +
                                e.EventDate.ToString("dd MMM yyyy HH:mm") + " — " +
                                e.Venue.VenueName
                    }).ToList(),
                "EventId", "Label", booking?.EventId
            );

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId)
                return NotFound();

            var selectedEvent = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventId == booking.EventId);

            if (selectedEvent == null)
            {
                ModelState.AddModelError("", "Selected event does not exist.");
            }
            else
            {
                booking.VenueId = selectedEvent.VenueId;

                // ✅ Check for existing bookings at same venue and time, excluding this booking
                bool isDoubleBooked = await _context.Bookings
                    .AnyAsync(b =>
                        b.BookingId != booking.BookingId && // exclude current booking
                        b.VenueId == booking.VenueId &&
                        b.BookingDate == booking.BookingDate);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked at the selected date and time.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.BookingId == id))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["EventId"] = new SelectList(
                _context.Events.Include(e => e.Venue)
                    .Select(e => new
                    {
                        e.EventId,
                        Label = e.EventName + " — " +
                                e.EventDate.ToString("dd MMM yyyy HH:mm") + " — " +
                                e.Venue.VenueName
                    }).ToList(),
                "EventId", "Label", booking?.EventId
            );

            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null) _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
