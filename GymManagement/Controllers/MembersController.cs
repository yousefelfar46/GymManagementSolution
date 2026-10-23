using GymManagement.BLL.Services.Attachment;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class MembersController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttachmentService _attachmentService;

        public MembersController(IMemberService memberService, IAttachmentService attachmentService)
        {
            _memberService = memberService;
            _attachmentService = attachmentService;
        }

        #region Picture : Get Member Picture


        [HttpGet]
        public async Task<IActionResult> Picture(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);

            if (member is null || string.IsNullOrWhiteSpace(member.Photo))
                return NotFound("Member or photo not found.");

            var result = _attachmentService.GetFile(member.Photo, "MembersPhotos");

            if (result is null)
                return NotFound($"Photo file not found: {member.Photo}");

            return File(result.Value.stream, result.Value.contentType);
        }

        #endregion

        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetAllMembersAsync();


            return View(members);
        }
    
        #region Create

        [HttpGet]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel member)
        {
            if (!ModelState.IsValid) return View(nameof(Create), member);

            var result = await _memberService.CreateMemberAsync(member);

            if (result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Create Member (Email or Phone may already exist)";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion
        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var result = await _memberService.GetHealthRecordAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessage"] = "HealthRecord not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(result);

        }
        #region Update

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel member, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(member);

            var result = await _memberService.UpdateMemberDetailsAsync(id, member, ct);


            if (result)
            {
                TempData["SuccessMessage"] = "Member Updated Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed To  Update Member  ";
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion
        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result)
            {
                TempData["SuccessMessage"] = "Member Deleted Successfully   ";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed To Delete Member  ";
            }
            return RedirectToAction(nameof(Index));

        }
        #endregion

    }
}
