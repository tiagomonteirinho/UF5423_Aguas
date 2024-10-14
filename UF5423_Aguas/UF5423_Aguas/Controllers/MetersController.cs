using System;
using System.Collections.Generic;
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

        [Authorize(Roles = "Employee")]
        public IActionResult Index()
        {
            return View(_meterRepository.GetAll().Include(m => m.User).OrderBy(m => m.User.FullName).ThenByDescending(m => m.Id));
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ListCustomers()
        {
            var users = await _userHelper.GetAllUsersAsync();
            var customers = new List<User>();
            foreach (var user in users)
            {
                user.RoleName = (await _userHelper.GetUserRolesAsync(user)).FirstOrDefault();
                if (user.RoleName == "Customer")
                {
                    customers.Add(user);
                }
            }

            return View(customers);
        }

        public async Task<IActionResult> ListConsumptions()
        {
            var model = await _meterRepository.GetConsumptionsAsync(this.User.Identity.Name); // Get consumptions depending on authenticated user.
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new {entityName = "Meter"});
            }

            var meter = await _meterRepository.GetMeterWithConsumptionsAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            return View(meter);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CustomerDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userHelper.GetUserByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            return View(user);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Create()
        {
            return View(new MeterViewModel
            {
                Users = await _userHelper.GetComboUsers()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterViewModel model)
        {
            model.Users = await _userHelper.GetComboUsers(); // Repopulate view users.
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
                user.Meters.Add(meter);
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
                    Users = await _userHelper.GetComboUsers() // Keep view users.
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
                var user = await _userHelper.GetUserByEmailAsync(meter.UserEmail);
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

        public async Task<IActionResult> AddConsumption(int? id)
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

            var model = new ConsumptionViewModel
            {
                MeterId = meter.Id,
                Meter = meter,
            };

            return View(model);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> AddConsumption(ConsumptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _meterRepository.AddConsumptionAsync(model);
                return RedirectToAction("Details", new { id = model.MeterId });
            }

            return View(model);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EditConsumption(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            var consumption = await _meterRepository.GetConsumptionAsync(id.Value);
            if (consumption == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            return View(consumption);
        }

        [HttpPost]
        public async Task<IActionResult> EditConsumption(Consumption consumption)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var meterId = await _meterRepository.UpdateConsumptionAsync(consumption);
                    if (meterId != 0)
                    {
                        return RedirectToAction("Details", new { id = meterId });
                    }

                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UPDATE"))
                    {
                        return RedirectToAction("Error", "Errors", new
                        {
                            title = $"Consumption update error.",
                            message = $"Could not update consumption. Please ensure that it is not being used by other entities.",
                        });
                    }

                    return RedirectToAction("Error", "Errors");
                }
            }

            return View(consumption);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteConsumption(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            var consumption = await _meterRepository.GetConsumptionAsync(id.Value);
            if (consumption == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            try
            {
                var meterId = await _meterRepository.DeleteConsumptionAsync(consumption);

                var referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer); // Redirect to previous page.
                }

                return RedirectToAction($"Details", new { id = meterId });
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    return RedirectToAction("Error", "Errors", new
                    {
                        title = $"Consumption deletion error.",
                        message = $"Could not remove consumption. Please ensure that it is not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }
    }
}
