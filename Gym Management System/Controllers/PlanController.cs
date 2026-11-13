using Gym_Management_System.DbContexts;
using Gym_Management_System.Models;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManangement.DAL.Repositories.Interfaces;
using GymManangement.DAL.Repositories.Interfaces.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_Management_System.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService planService;

        public PlanController(IPlanService planService)
        {
            this.planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken=default)
        {
            var plan = await planService.GetAllPlansAsync(cancellationToken);
            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id,CancellationToken cancellationToken)
        {
            var plan = await planService.GetPlanByIdAsync(id, cancellationToken);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {
            var plan = await planService.GetPlanToUpdateAsync(id,ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan cannot be edited (not found, inactive, or has active memberships).";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id,UpdatePlanViewModel updatePlanViewModel,CancellationToken ct)
        {
            var result = await planService.UpdatePlanAsync(id, updatePlanViewModel, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            return View(updatePlanViewModel);

        }
        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct)
        {
            var result = await planService.ToggleActivationAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Plan status changed." : result.Error;
            return RedirectToAction(nameof(Index));
        }

    }

    }

