using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;

namespace UF5423_Aguas.Controllers
{
    public class MetersController : Controller
    {
        private readonly DataContext _context;

        public MetersController(DataContext context)
        {
            _context = context;
        }

        // GET: Meters
        public async Task<IActionResult> Index()
        {
            return View(await _context.Meters.ToListAsync());
        }

        // GET: Meters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meter = await _context.Meters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meter == null)
            {
                return NotFound();
            }

            return View(meter);
        }

        // GET: Meters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Meters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meter meter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(meter);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(meter);
        }

        // GET: Meters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meter = await _context.Meters.FindAsync(id);
            if (meter == null)
            {
                return NotFound();
            }
            return View(meter);
        }

        // POST: Meters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meter meter)
        {
            if (id != meter.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(meter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeterExists(meter.Id))
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
            return View(meter);
        }

        // GET: Meters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meter = await _context.Meters
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meter == null)
            {
                return NotFound();
            }

            return View(meter);
        }

        // POST: Meters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meter = await _context.Meters.FindAsync(id);
            _context.Meters.Remove(meter);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeterExists(int id)
        {
            return _context.Meters.Any(e => e.Id == id);
        }
    }
}
