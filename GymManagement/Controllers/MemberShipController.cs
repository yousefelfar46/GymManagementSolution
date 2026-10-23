using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class MemberShipController(IMembershipService MembershipService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            return View(await MembershipService.GetAllMemberShipAsync(cancellationToken));
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            await PopulateDropdownsAsync(cancellationToken);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(model);
            }

            var result = await MembershipService.CreateMembershipAsync(model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Error;
            await PopulateDropdownsAsync(ct);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, CancellationToken ct)
        {
            var result = await MembershipService.DeleteActiveMembership(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Membership Cancelled" : result.Error;

            return RedirectToAction(nameof(Index));
        }
        private async Task PopulateDropdownsAsync(CancellationToken ct = default)
        {
            ViewBag.Members = new SelectList(await MembershipService.GetSelectListViewsAsync(ct), "Id", "Name");
            ViewBag.Plans = new SelectList(await MembershipService.GetPlanSelectListViewsAsync(ct), "Id", "Name");
        }
    }
} 
