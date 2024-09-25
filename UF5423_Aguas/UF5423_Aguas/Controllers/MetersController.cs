using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;

namespace UF5423_Aguas.Controllers
{
    public class MetersController : Controller
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IUserHelper _userHelper;

        public MetersController(IMeterRepository meterRepository, IUserHelper userHelper)
        {
            _meterRepository = meterRepository;
            _userHelper = userHelper;
        }

        // GET: Meters
        public IActionResult Index()
        {
            return View(_meterRepository.GetAll().OrderBy(m => m.Name));
        }

        // GET: Meters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meter = await _meterRepository.GetByIdAsync(id.Value);
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
                //meter.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                meter.User = await _userHelper.GetUserByEmailAsync("admin@mail.com");
                await _meterRepository.CreateAsync(meter);
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

            var meter = await _meterRepository.GetByIdAsync(id.Value);
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
                    //meter.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    meter.User = await _userHelper.GetUserByEmailAsync("admin@mail.com");
                    await _meterRepository.UpdateAsync(meter);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _meterRepository.ExistsAsync(meter.Id))
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

            var meter = await _meterRepository.GetByIdAsync(id.Value);
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
            var meter = await _meterRepository.GetByIdAsync(id);
            await _meterRepository.DeleteAsync(meter);
            return RedirectToAction(nameof(Index));
        }
    }
}
