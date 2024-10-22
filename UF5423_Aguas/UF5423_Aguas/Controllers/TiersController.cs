using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using UF5423_Aguas.Data;
using UF5423_Aguas.Data.Entities;
using UF5423_Aguas.Helpers;
using UF5423_Aguas.Models;

namespace UF5423_Aguas.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TiersController : Controller
    {
        private readonly ITierRepository _tierRepository;
        private readonly DataContext _context;

        public TiersController(ITierRepository tierRepository, DataContext context)
        {
            _tierRepository = tierRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            var model = _tierRepository.GetAll();
            return View(model);
        }

        public IActionResult Create()
        {
            return View(new TierViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TierViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not add tier.";
                return View();
            }

            var tier = new Tier()
            {
                UnitPrice = model.UnitPrice,
                VolumeLimit = model.VolumeLimit,
            };

            await _tierRepository.CreateAsync(tier);
            await _tierRepository.SaveAllAsync();
            if (!await _tierRepository.ExistsAsync(tier.Id))
            {
                ViewBag.ErrorMessage = "Could not add tier.";
                return View(model);
            }

            ViewBag.SuccessMessage = "Tier added successfully!";
            return View();
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            var tier = await _tierRepository.GetByIdAsync(id.Value);
            if (tier == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            return View(new TierViewModel
            {
                Id = tier.Id,
                UnitPrice = tier.UnitPrice,
                VolumeLimit = tier.VolumeLimit,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, TierViewModel model)
        {
            if (id == null || model.Id != id)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            var tier = await _tierRepository.GetByIdAsync(id.Value);
            if (tier == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Could not update tier.";
                return View(model);
            }

            try
            {
                tier.UnitPrice = model.UnitPrice;
                tier.VolumeLimit = model.VolumeLimit;

                await _tierRepository.UpdateAsync(tier);
                ViewBag.SuccessMessage = "Tier updated successfully!";
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("UPDATE"))
                {
                    return RedirectToAction("Error", "Errors", new
                    {
                        title = $"Tier update error.",
                        message = $"Could not update tier. Please ensure that it is not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            var tier = await _tierRepository.GetByIdAsync(id.Value);
            if (tier == null)
            {
                return RedirectToAction("NotFound404", "Errors", new { entityName = "Tier" });
            }

            try
            {
                await _tierRepository.DeleteAsync(tier);
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    return RedirectToAction("Error", "Errors", new
                    {
                        title = $"Tier deletion error.",
                        message = $"Could not remove tier. Please ensure that it is not being used by other entities.",
                    });
                }

                return RedirectToAction("Error", "Errors");
            }
        }
    }
}
