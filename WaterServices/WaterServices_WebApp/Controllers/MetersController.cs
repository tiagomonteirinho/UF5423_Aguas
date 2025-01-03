﻿using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterServices_WebApp.Data;
using WaterServices_WebApp.Data.Entities;
using WaterServices_WebApp.Helpers;
using WaterServices_WebApp.Models;

namespace WaterServices_WebApp.Controllers
{
    [Authorize(Roles = "Admin, Employee, Customer")]
    public class MetersController : Controller
    {
        private readonly IMeterRepository _meterRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserHelper _userHelper;
        private readonly DataContext _context;
        private readonly IPaymentHelper _paymentHelper;

        public MetersController(IMeterRepository meterRepository, INotificationRepository notificationRepository, IUserHelper userHelper, DataContext context, IPaymentHelper paymentHelper)
        {
            _meterRepository = meterRepository;
            _notificationRepository = notificationRepository;
            _userHelper = userHelper;
            _context = context;
            _paymentHelper = paymentHelper;
        }

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Employee, Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var meter = await _meterRepository.GetMeterWithAllRelatedDataAsync(id.Value);
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
        public async Task<IActionResult> Create(string userId)
        {
            if (userId == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var model = new MeterViewModel
            {
                UserId = userId,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not add meter.";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.UserId))
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var user = await _userHelper.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "User" });
            }

            var random = new Random();
            var meter = new Meter()
            {
                SerialNumber = model.SerialNumber,
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

            user.Meters.Add(meter);

            var meterUrl = Url.Action
            (
                "Details",
                "Meters",
                new { id = meter.Id },
                protocol: HttpContext.Request.Scheme,
                host: AppConfig.Host
            );

            var notification = new Notification
            {
                Title = "New water meter added successfully.",
                Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                Receiver = user,
                ReceiverEmail = user.Email,
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Meter added successfully!";
            ModelState.Clear(); // Clear view form.
            return View(new MeterViewModel
            {
                UserId = model.UserId
            });
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
        public async Task<IActionResult> Edit(int? id, MeterViewModel model)
        {
            if (id == null || model.Id != id)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var meter = await _meterRepository.GetByIdAsync(id.Value);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    meter.Address = model.Address;

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
            var meter = await _meterRepository.GetMeterWithAllRelatedDataAsync(id);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            try
            {
                await _meterRepository.DeleteAsync(meter);
                return RedirectToAction($"CustomerDetails", new { id = meter.User.Id });
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
            catch (InvalidOperationException)
            {
                return RedirectToAction("Error", "Errors", new
                {
                    title = $"Meter deletion error.",
                    message = $"Could not remove meter. Please ensure that it is not being used by other entities.",
                });
            }
        }

        [Authorize(Roles = "Employee, Customer")]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddConsumption(ConsumptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not add consumption.";
                return View(model);
            }

            var consumption = await _meterRepository.AddConsumptionAsync(model);
            await _context.SaveChangesAsync();

            var meter = await _meterRepository.GetMeterWithUserByIdAsync(model.MeterId);
            if (meter == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Meter" });
            }

            var userEmail = User.Identity.Name;
            var user = await _userHelper.GetUserByEmailAsync(userEmail);
            var meterUrl = Url.Action
            (
                "Details",
                "Meters",
                new { id = meter.Id },
                protocol: HttpContext.Request.Scheme,
                host: AppConfig.Host
            );

            if (await _userHelper.IsUserInRoleAsync(user, "Customer"))
            {
                var roleNotification = new Notification
                {
                    Title = "Consumption awaiting approval.",
                    Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                    ReceiverRole = "Employee"
                };

                await _notificationRepository.CreateAsync(roleNotification);
            }
            else
            {
                await _meterRepository.ApproveConsumption(consumption);
                var userNotification = new Notification
                {
                    Title = "Consumption awaiting payment.",
                    Action = $"<a href=\"{meterUrl}\" class=\"btn btn-primary\">Go to meter</a>",
                    Receiver = meter.User,
                    ReceiverEmail = meter.User.Email,
                };

                await _notificationRepository.CreateAsync(userNotification);
            }

            await _context.SaveChangesAsync();

            ViewBag.SuccessMessage = "Consumption added successfully!";
            return View(new ConsumptionViewModel
            {
                MeterId = model.MeterId
            });
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

            return View(new ConsumptionViewModel
            {
                Id = consumption.Id,
                MeterId = consumption.MeterId,
                Volume = consumption.Volume,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConsumption(int? id, ConsumptionViewModel model)
        {
            if (id == null || model.Id != id)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            var consumption = await _meterRepository.GetConsumptionByIdAsync(id.Value);
            if (consumption == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Consumption" });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    consumption.Volume = model.Volume;

                    await _meterRepository.UpdateConsumptionAsync(consumption);
                    ViewBag.SuccessMessage = "Consumption updated successfully!";
                    return View(model);
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

            ViewBag.ErrorMessage = "Could not update consumption.";
            return View(model);
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
                Title = "New contract awaiting approval.",
                Message =
                $"<h5>Contract holder</h5>" +
                $"<p>{model.FullName}</p>" +
                $"<h5>E-mail</h5>" +
                $"<p>{model.Email}</p>" +
                $"<h5>Phone contact</h5>" +
                $"<p>{model.PhoneNumber}</p>" +
                $"<h5>Address</h5>" +
                $"<p>{model.Address}</p>" +
                $"<h5>Postal code</h5>" +
                $"<p>{model.PostalCode}</p>" +
                $"<h5>Serial number</h5>" +
                $"<p>{model.SerialNumber}</p>",
                ReceiverRole = "Employee",
                NewAccountEmail = model.Email,
            };

            await _notificationRepository.CreateAsync(notification);
            notification.Action = $"<a href=\"{Url.Action("ForwardNotificationToAdmin", "Meters", new { id = notification.Id })}\" class=\"btn btn-primary\">Forward to administrator</a>";
            await _notificationRepository.UpdateAsync(notification);

            ViewBag.SuccessMessage = "Meter contract request submitted successfully!";
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

            await _notificationRepository.CreateAsync(forwardedNotification);
            forwardedNotification.Action = $"<p style=\"color:gray\">*Forward notification after creating customer account.</p>" +
                $"<a href=\"{Url.Action("ForwardNotificationToEmployee", "Meters", new { id = forwardedNotification.Id })}\" class=\"btn btn-primary\">Forward to employee</a>";
            await _notificationRepository.UpdateAsync(forwardedNotification);

            if (!notification.Action.Contains("Notification forwarded successfully!"))
            {
                notification.Action += "Notification forwarded successfully!";
            }

            await _notificationRepository.UpdateAsync(notification);
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

            var userUrl = Url.Action
            (
                "CustomerDetails",
                "Meters",
                new { id = user.Id },
                protocol: HttpContext.Request.Scheme,
                host: AppConfig.Host
            );

            var forwardedNotification = new Notification
            {
                Title = "New customer contract confirmed and awaiting water meter addition.",
                Message = notification.Message,
                Action = $"<a href=\"{userUrl}\" class=\"btn btn-primary\">Go to user</a>",
                ReceiverRole = "Employee",
            };

            await _notificationRepository.CreateAsync(forwardedNotification);

            if (!notification.Action.Contains("Notification forwarded successfully!"))
            {
                notification.Action += "Notification forwarded successfully!";
            }

            await _notificationRepository.UpdateAsync(notification);
            return RedirectToAction("NotificationDetails", "Users", new { id = notification.Id });
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApproveConsumption(int? id)
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

            if (consumption.Status != "Awaiting approval")
            {
                return RedirectToAction("Error", "Errors");
            }

            await _meterRepository.ApproveConsumption(consumption);
            return RedirectToAction("Details", "Meters", new { id = consumption.MeterId });
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> BuyConsumption(int? id)
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

            var invoice = await _meterRepository.GetInvoiceByConsumptionIdAsync(consumption.Id);
            if (invoice == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Invoice" });
            }

            var gateway = _paymentHelper.GetGateway();
            var clientToken = gateway.ClientToken.Generate();
            ViewBag.ClientToken = clientToken;
            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> BuyConsumption(Invoice model)
        {
            var consumption = await _meterRepository.GetConsumptionByIdAsync(model.ConsumptionId);
            var gateway = _paymentHelper.GetGateway();
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(model.Price),
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            if (!result.IsSuccess())
            {
                TempData["ErrorMessage"] = "Could not accept payment.";
                return RedirectToAction("Details", "Meters", new { id = consumption.MeterId });
            }

            consumption.Status = "Payment confirmed";
            await _meterRepository.UpdateConsumptionAsync(consumption);
            TempData["SuccessMessage"] = "Payment accepted successfully!";
            return RedirectToAction("Details", "Meters", new { id = consumption.MeterId });
        }
    }
}
