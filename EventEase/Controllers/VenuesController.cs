using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using EventEase.Data;
using EventEase.Models;
using EventEase.Services; // 👈 Needed for BlobService
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseDbContext _context;

        public VenuesController(EventEaseDbContext context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("VenueId,VenueName,Location,Capacity")] Venue venue,
            IFormFile imageFile,
            [FromServices] BlobService blobService)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    using var stream = imageFile.OpenReadStream();
                    venue.ImageUrl = await blobService.UploadFileAsync(stream, fileName);
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Venue created successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("VenueId,VenueName,Location,Capacity,ImageUrl")] Venue venue)
        {
            if (id != venue.VenueId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Venue updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue == null)
                return NotFound();

            // Check if any bookings exist for this venue
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            bool hasEvents = await _context.Events.AnyAsync(e => e.VenueId == id);

            if (hasBookings || hasEvents)
            {
                TempData["ErrorMessage"] = "Cannot delete this venue because it is associated with existing events or bookings.";
                return RedirectToAction(nameof(Index));
            }

            _context.Venues.Remove(venue);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Venue deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(v => v.VenueId == id);
        }


    }
}

