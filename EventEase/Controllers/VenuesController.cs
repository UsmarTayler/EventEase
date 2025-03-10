using Microsoft.AspNetCore.Mvc;
using EventEase.Data;
using EventEase.Models;
using System.Linq;

namespace EventEase.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseDbContext _context;

        public VenuesController(EventEaseDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Venues.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                _context.Venues.Add(venue);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }
    }
}

