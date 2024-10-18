using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Controllers
{
    [Authorize(Roles = "Admin, Employee, Customer")]
    public class MetersController : Controller
    {
        private readonly IMeterRepository _meterRepository;
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;

        public MetersController(IMeterRepository meterRepository, IUserHelper userHelper, DataContext context)
        {
            _meterRepository = meterRepository;
            _userHelper = userHelper;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _meterRepository.GetMetersAsync(User.Identity.Name);
            return View(model);
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
            var model = await _meterRepository.GetConsumptionsAsync(User.Identity.Name);
            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var meter = await _meterRepository.GetMeterWithConsumptionsAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var email = User.Identity.Name;
            if (email != meter.UserEmail && !User.IsInRole("Employee"))
            {
                return RedirectToAction("Unauthorized401", "Errors");
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

                var random = new Random();
                var meter = new Meter()
                {
                    Address = model.Address,
                    UserEmail = user.Email,
                    User = user,
                    SerialNumber = random.Next(1000000, 10000000),
                };

                await _meterRepository.CreateAsync(meter);
                await _meterRepository.SaveAllAsync();
                if (!await _meterRepository.ExistsAsync(meter.Id))
                {
                    ViewBag.ErrorMessage = "Could not add meter.";
                    return View(model);
                }

                user.Meters.Add(meter);
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
                SerialNumber = meter.SerialNumber,
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
                    meter.SerialNumber = model.SerialNumber;
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
            var meter = await _meterRepository.GetMeterWithConsumptionsAsync(id);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            if (meter.Consumptions.Any())
            {
                return RedirectToAction("Error", "Errors", new
                {
                    title = $"Meter deletion error.",
                    message = $"Could not remove meter. Please ensure that it is not being used by other entities.",
                });
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
                Date = DateTime.Now,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddConsumption(ConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var meter = await _meterRepository.GetMeterWithUserByIdAsync(model.MeterId);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            await _meterRepository.AddConsumptionAsync(model);
            var userEmail = User.Identity.Name;
            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            var meterUrl = Url.Action("Details", "Meters", new { id = meter.Id }, protocol: HttpContext.Request.Scheme);
            if (await _userHelper.IsUserInRoleAsync(user, "Customer"))
            {
                var roleNotification = new Notification
                {
                    Title = "New water meter consumption awaiting validation.",
                    Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                    ReceiverRole = "Employee"
                };

                _context.Notifications.Add(roleNotification);
            }
            else
            {
                var userNotification = new Notification
                {
                    Title = "New water meter consumption invoice awaiting payment.",
                    Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                    Receiver = meter.User,
                    ReceiverEmail = meter.User.Email,
                };

                _context.Notifications.Add(userNotification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = model.MeterId });
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> EditConsumption(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            var consumption = await _meterRepository.GetConsumptionByIdAsync(id.Value);
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

            var consumption = await _meterRepository.GetConsumptionByIdAsync(id.Value);
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

        [AllowAnonymous]
        public IActionResult RequestMeter()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> RequestMeter(RequestMeterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not request meter.";
                return View(model);
            }

            var notification = new Notification
            {
                Title = "New water meter contract request awaiting validation",
                Message =
                $"<h5>Contract holder (full name)</h5>" +
                $"<p>{model.FullName}</p>" +
                $"<h5>E-mail</h5>" +
                $"<p>{model.Email}</p>" +
                $"<h5>Phone contact</h5>" +
                $"<p>{model.PhoneNumber}</p>" +
                $"<h5>Address</h5>" +
                $"<p>{model.Address}</p>" +
                $"<h5>Postal code</h5>" +
                $"<p>{model.PostalCode}</p>",
                ReceiverRole = "Employee",
                NewAccountEmail = model.Email,
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            notification.Action = $"<a href=\"{Url.Action("ForwardNotificationToAdmin", "Meters", new { id = notification.Id })}\" class=\"btn btn-primary\">Forward to administrator</a>";
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Meter request submitted successfully!";
            ModelState.Clear();
            return View();
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ForwardNotificationToAdmin(int id)
        {
            var notification = _context.Notifications
                .Where(n => n.Id == id)
                .FirstOrDefault();

            if (notification == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Notification" });
            }

            var forwardedNotification = new Notification
            {
                Title = "New customer awaiting account creation",
                Message = notification.Message,
                ReceiverRole = "Admin",
                NewAccountEmail = notification.NewAccountEmail,
            };

            _context.Notifications.Add(forwardedNotification);
            await _context.SaveChangesAsync();
            forwardedNotification.Action = $"<p style=\"color:gray\">*Forward notification after creating customer account.</p>" +
                $"<a href=\"{Url.Action("ForwardNotificationToEmployee", "Meters", new { id = forwardedNotification.Id })}\" class=\"btn btn-primary\">Forward to employee</a>";
            _context.Notifications.Update(forwardedNotification);

            if (!notification.Action.Contains("Notification forwarded successfully!"))
            {
                notification.Action += "Notification forwarded successfully!";
            }

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction("NotificationDetails", "Users", new { id = notification.Id });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ForwardNotificationToEmployee(int id)
        {
            var notification = _context.Notifications
                .Where(n => n.Id == id)
                .FirstOrDefault();

            if (notification == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Notification" });
            }

            User user = await _userHelper.GetUserByEmailAsync(notification.NewAccountEmail);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var userUrl = Url.Action("CustomerDetails", "Meters", new { id = user.Id }, protocol: HttpContext.Request.Scheme);
            var forwardedNotification = new Notification
            {
                Title = "New customer contract confirmed and awaiting water meter addition",
                Message = notification.Message,
                Action = $"<a href=\"{userUrl}\" class=\"btn btn-primary\">Go to user</a>",
                ReceiverRole = "Employee",
            };

            _context.Notifications.Add(forwardedNotification);
            await _context.SaveChangesAsync();

            if (!notification.Action.Contains("Notification forwarded successfully!"))
            {
                notification.Action += "Notification forwarded successfully!";
            }

            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
            return RedirectToAction("NotificationDetails", "Users", new { id = notification.Id });
        }
    }
}
