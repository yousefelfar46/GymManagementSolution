using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagement.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _uow;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            ViewBag.TotalMembers = (await _uow.GetRepository<Member>().GetAllAsync(ct: ct)).Count();
            ViewBag.TotalTrainers = (await _uow.GetRepository<Trainer>().GetAllAsync(ct: ct)).Count();
            ViewBag.UpcomingSessions = (await _uow.GetRepository<Session>().GetAllAsync(s => s.StartDate > DateTime.Now, ct: ct)).Count();
            ViewBag.CompletedSessions = (await _uow.GetRepository<Session>().GetAllAsync(s => s.EndDate < DateTime.Now, ct: ct)).Count();
            ViewBag.OngoingSessions = (await _uow.GetRepository<Session>().GetAllAsync(s => s.StartDate <= DateTime.Now && s.EndDate >= DateTime.Now, ct: ct)).Count();
            return View();
        }
    }
}
