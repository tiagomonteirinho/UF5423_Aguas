using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;

namespace UF5423_Aguas.Controllers
{
    [Authorize(Roles = "Employee, Customer")]
    public class MetersController : Controller
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IUserHelper _userHelper;

        public MetersController(IMeterRepository meterRepository, IUserHelper userHelper)
        {
            _meterRepository = meterRepository;
            _userHelper = userHelper;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _meterRepository.GetMetersAsync(this.User.Identity.Name); // Get meters depending on authenticated user.
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new {entityName = "Meter"});
            }

            var meter = await _meterRepository.GetByIdAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            return View(meter);
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meter meter)
        {
            if (ModelState.IsValid)
            {
                meter.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _meterRepository.CreateAsync(meter);
                return RedirectToAction("Index");
            }

            return View(meter);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var meter = await _meterRepository.GetByIdAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            return View(meter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meter meter)
        {
            if (id != meter.Id)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    meter.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _meterRepository.UpdateAsync(meter);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UPDATE"))
                    {
                        return RedirectToAction("Error", "Errors", new
                        {
                            title = $"Meter update error.",
                            message = $"Meter {meter.Name} could not be updated. Please ensure that it is not being used by other entities.",
                        });
                    }

                    return RedirectToAction("Error", "Errors");
                }
            }

            return View(meter);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = $"Meter" });
            }

            var meter = await _meterRepository.GetByIdAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            return View(meter);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meter = await _meterRepository.GetByIdAsync(id);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            try
            {
                await _meterRepository.DeleteAsync(meter);
                return RedirectToAction("Index");

            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    return RedirectToAction("Error", "Errors", new
                    {
                        title = $"Meter deletion error.",
                        message = $"Meter {meter.Name} could not be deleted. Please ensure that it is not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }
    }
}
