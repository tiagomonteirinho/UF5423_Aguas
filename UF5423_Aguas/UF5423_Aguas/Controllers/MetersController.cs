using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

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
            return View(new MeterViewModel
            {
                Users = _userHelper.GetComboUsers()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterViewModel model)
        {
            model.Users = _userHelper.GetComboUsers(); // Repopulate view users.
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserEmail);
                if (user == null)
                {
                    return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
                }

                var meter = new Meter
                {
                    Name = model.Name,
                    Address = model.Address,
                    UserEmail = user.Email,
                    User = user,
                };

                await _meterRepository.CreateAsync(meter);
                await _meterRepository.SaveAllAsync();
                if (!await _meterRepository.ExistsAsync(meter.Id))
                {
                    ViewBag.ErrorMessage = "Could not add meter.";
                    return View(model);
                }
                
                ViewBag.SuccessMessage = "Meter added successfully!";
                ModelState.Clear(); // Clear view form.
                return View(new MeterViewModel
                {
                    Users = _userHelper.GetComboUsers() // Keep view users.
                });
            }

            ViewBag.ErrorMessage = "Could not add meter.";
            return View(model);
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

            return View(new MeterViewModel
            {
                Id = meter.Id,
                Name = meter.Name,
                Address = meter.Address,
                UserEmail = meter.UserEmail,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MeterViewModel model)
        {
            var meter = await _meterRepository.GetByIdAsync(id);
            if (meter == null || id != model.Id)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    meter.Name = model.Name;
                    meter.Address = model.Address;
                    meter.UserEmail = model.UserEmail;

                    await _meterRepository.UpdateAsync(meter);
                    ViewBag.SuccessMessage = "Meter updated successfully!";
                    return View(model);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UPDATE"))
                    {
                        return RedirectToAction("Error", "Errors", new
                        {
                            title = $"Meter update error.",
                            message = $"Could not update meter. Please ensure that it is not being used by other entities.",
                        });
                    }

                    return RedirectToAction("Error", "Errors");
                }
            }

            ViewBag.ErrorMessage = "Could not update meter.";
            return View(model);
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
                        message = $"Could not remove meter. Please ensure that it is not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }
    }
}
