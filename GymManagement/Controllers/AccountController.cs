using GymManagement.BLL.ViewModels.AccountViewModel;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVm)
        {
            if (!ModelState.IsValid) return View(loginVm);
            var user = await _userManager.FindByEmailAsync(loginVm.Email);
            if (user == null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(loginVm);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVm.Password, loginVm.RememberMe, false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                _logger.LogWarning($"User account locked out: {user.Email}");
                ModelState.AddModelError("InvalidLogin", "This Account Is Locked Now ");
                return View(loginVm);

            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid Email or Password");
                return View(loginVm);
            }

        }


        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

     

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
