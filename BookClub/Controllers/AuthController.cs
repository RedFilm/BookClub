using BookClub.Models;
using BookClub.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace BookClub.Controllers
{
    public class AuthController : Controller
    {
        private SignInManager<AppUser> _signInMnr;
        private UserManager<AppUser> _userMnr;
        private RoleManager<IdentityRole> _roleMnr;

        public AuthController(SignInManager<AppUser> signInMnr,
                              UserManager<AppUser> userMnr,
                              RoleManager<IdentityRole> roleMnr)
        {
            _signInMnr = signInMnr;
            _userMnr = userMnr;
            _roleMnr = roleMnr;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());
       
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var userRole = new IdentityRole("User");
            var existedUser = await _userMnr.FindByNameAsync(vm.UserName) ?? await _userMnr.FindByEmailAsync(vm.Email);

            if (existedUser != null)
            {
                return BadRequest("User with the same name or email already exists");
            }

            var user = new AppUser()
            {
                UserName = vm.UserName,
                Email = vm.Email,
            };

            var result = await _userMnr.CreateAsync(user, vm.Password);

            if (!result.Succeeded)
                return BadRequest("Something went wrong");

            if (!await _roleMnr.RoleExistsAsync(userRole.Name))
                await _roleMnr.CreateAsync(userRole);

            await _userMnr.AddToRoleAsync(user, userRole.Name);

            await _signInMnr.SignInAsync(user, isPersistent: false);

            return Ok("You're have been registred!");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userMnr.FindByEmailAsync(vm.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var result = await _signInMnr.PasswordSignInAsync(user, vm.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInMnr.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
