
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading;
using System.Threading.Tasks;

namespace Gym_Management_System.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionService sessionService;

        public SessionController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var sessions = await sessionService.GetAllSessions(cancellationToken);
            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct = default)
        {
            await PopulateDropdownsAsync(ct);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateSessionViewModel createSessionViewModel,
            CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(cancellationToken);
                return View(createSessionViewModel);
            }

            var result = await sessionService.CreateSessionAsync(
                createSessionViewModel,
                cancellationToken);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Failed to create session. Please check the input values.");

                await PopulateDropdownsAsync(cancellationToken);
                return View(createSessionViewModel);
            }

            TempData["SuccessMessage"] = "Session created successfully.";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id,CancellationToken ct)
        {

            var session = await sessionService.GetSessionById(id, ct);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id,CancellationToken ct)
        {

            var session = await sessionService.GetSessionToUpdateAsync(id,ct);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session cannot be edited (not found, already started, or has bookings).";
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(ct);
            return View(session);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(model);
            }

            var result = await sessionService.UpdateSessionAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropdownsAsync(ct);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await sessionService.GetSessionById(id, ct);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await sessionService.RemoveSessionAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Session deleted successfully." : result.Error;
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(CancellationToken ct)
        {
            var trainers = await sessionService.GetTrainerFromDropDownAsync(ct);
            var categories = await sessionService.GetCategoryFromDropDownAsync(ct);

            ViewBag.Trainers = new SelectList(
                trainers ?? Enumerable.Empty<TrainerSelectViewModel>(),
                "Id",
                "Name");

            ViewBag.Categories = new SelectList(
                categories ?? Enumerable.Empty<CategorySelectViewModel>(),
                "Id",
                "CategoryName");
        }

    }
}
