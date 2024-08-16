using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OneTrack.ViewModels;

namespace OneTrack.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public RoleController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // List all roles and the users in each role
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = _roleManager.Roles.ToList();
            var roleUsersViewModel = new List<RoleUsersViewModel>();

            foreach (var role in roles)
            {
                var thisViewModel = new RoleUsersViewModel
                {
                    RoleName = role.Name,
                    Users = await GetUsersInRole(role.Name)
                };

                roleUsersViewModel.Add(thisViewModel);
            }

            ViewData["Users"] = _userManager.Users.ToList();
            return View(roleUsersViewModel);
        }        

        // Create new role
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(roleName))
            {
                // Check if the user is already in the role
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    var result = await _userManager.AddToRoleAsync(user, roleName);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User is already in this role.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user or role.");
            }

            return RedirectToAction("Index");
        }

        // Remove a role from a user
        [HttpPost]
        public async Task<IActionResult> RemoveRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null &&  !string.IsNullOrEmpty(roleName))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (result.Succeeded) 
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return RedirectToAction("Index");
        }


        private async Task<List<IdentityUser>> GetUsersInRole(string roleName)
        {
            var users = new List<IdentityUser>();
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, roleName))
                {
                    users.Add(user);
                }
            }

            return users;
        }
    }
}
