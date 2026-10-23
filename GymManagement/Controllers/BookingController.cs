using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _bookingService.GetAllSessionsAsync(ct);

            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForUpComingSession(int sessionId, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForUpComingSessionByIdAsync(sessionId, ct);
            ViewBag.SessionId = sessionId;
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSession(int sessionId, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForOnComingSessionByIdAsync(sessionId, ct);

            return View(members);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int SessionId, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropdownAsync(SessionId, ct);

            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = SessionId;
            var model = new CreateBookingViewModel { SessionId = SessionId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            var result = await _bookingService.CreateNewBookingAsync(model, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Booking Created Successfully" : result.Error;

            return RedirectToAction(nameof(GetMembersForUpComingSession), new { sessionId = model.SessionId });
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Booking Cancelled Successfully" : result.Error;

            return RedirectToAction(nameof(GetMembersForUpComingSession), new { SessionId = sessionId });
        }
        [HttpPost]
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAsAttend(memberId, sessionId, ct);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success ? "Attenddance Recorded" : result.Error;

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { SessionId = sessionId });
        }
    }
}
