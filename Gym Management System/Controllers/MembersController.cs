
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.Services.Interfaces.Attachement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Gym_Management_System.Controllers
{
    public class MembersController : Controller
    {
        private readonly IMemberService memberService;
        private readonly IAttachementService attachementService;

        public MembersController(
            IMemberService memberService,
            IAttachementService attachementService)
        {
            this.memberService = memberService;
            this.attachementService = attachementService;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var members = await memberService.GetAllMembersAsync(ct);
            return View(members);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateMemberViewModel createMemberViewModel,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(createMemberViewModel);

            if (createMemberViewModel.PhotoFile == null ||
                createMemberViewModel.PhotoFile.Length == 0)
            {
                ModelState.AddModelError(
                    nameof(createMemberViewModel.PhotoFile),
                    "Please select a photo.");

                return View(createMemberViewModel);
            }

            string? photoName;

            await using (var stream = createMemberViewModel.PhotoFile.OpenReadStream())
            {
                photoName = await attachementService.UploadAsync(
                    stream,
                    createMemberViewModel.PhotoFile.FileName,
                    "MemeberPicture",
                    ct);
            }

            if (photoName == null)
            {
                ModelState.AddModelError(
                    nameof(createMemberViewModel.PhotoFile),
                    "Photo upload failed. Use JPG, JPEG, or PNG under 5 MB.");

                return View(createMemberViewModel);
            }

            createMemberViewModel.Photo = photoName;

            var result = await memberService.CreateMemberAsync(
                createMemberViewModel,
                ct);

            if (!result)
            {
                attachementService.Delete(photoName, "MemeberPicture");

                ModelState.AddModelError(
                    string.Empty,
                    "Email or phone number already exists.");

                return View(createMemberViewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MemberDetails(
            int id,
            CancellationToken ct = default)
        {
            var member = await memberService.GetMemberDetailsByIdAsync(id, ct);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        public async Task<IActionResult> HealthRecordDetails(
            int id,
            CancellationToken cancellationToken = default)
        {
            var result = await memberService.GetHealthRecordDetails(
                id,
                cancellationToken);

            if (result is null)
            {
                TempData["ErrorMessage"] = "Health record not found";
                return RedirectToAction(nameof(Index));
            }

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EditMember(
            int id,
            CancellationToken cancellationToken = default)
        {
            var member = await memberService.UpdateMember(id, cancellationToken);

            if (member is null)
                return RedirectToAction(nameof(Index));

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(
            int id,
            MemberToUpdateViewModel memberToUpdateViewModel,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(memberToUpdateViewModel);

            var result = await memberService.UpdateMemberDetailsAsync(
                id,
                memberToUpdateViewModel,
                ct);

            if (result)
                TempData["SuccessMessage"] = "Member is updated successfully";
            else
                TempData["ErrorMessage"] = "Member is not updated successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetPhoto(string fileName)
        {
            var file = attachementService.GetFile(fileName, "MemeberPicture");

            if (file is null)
                return NotFound();

            return File(file.Value.Stream, file.Value.ContentType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPhoto(
            IFormFile photoFile,
            CancellationToken ct = default)
        {
            if (photoFile == null || photoFile.Length == 0)
                return BadRequest("Please select a photo.");

            await using var stream = photoFile.OpenReadStream();

            var fileName = await attachementService.UploadAsync(
                stream,
                photoFile.FileName,
                "MemeberPicture",
                ct);

            if (fileName == null)
                return BadRequest("Photo upload failed.");

            return Ok(new { FileName = fileName });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            var member = await memberService.GetMemberDetailsByIdAsync(
                id,
                cancellationToken);

            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id,
            CancellationToken ct = default)
        {
            var result = await memberService.RemoveMemberAsync(id, ct);

            if (result)
                TempData["SuccessMessage"] = $"Member {id} deleted successfully.";
            else
                TempData["ErrorMessage"] = $"Member {id} was not deleted.";

            return RedirectToAction(nameof(Index));
        }
    }
}

