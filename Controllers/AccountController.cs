using DevExpress.XtraRichEdit.Import.Html;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class AccountController: Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _context = context;
    }
    public IActionResult Index()
    {
    return View();
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new user
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Check if roles exist; if not, create them (one time)
                if (!await _roleManager.RoleExistsAsync("Admin"))
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));

                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole("User"));

                // Assign role
                var userCount = await _userManager.Users.CountAsync();
                if (userCount == 1)
                {
                    // First user → Admin
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // All others → User
                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Display errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Login Successfully, {model.Email}!";
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                TempData["ErrorMessage"] = "Your account is locked out.";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid login attempt.";
                return RedirectToAction("Login");
            }
        }

        // If ModelState is invalid
        TempData["ErrorMessage"] = "Please provide valid login credentials.";
        return RedirectToAction("Login");
    }
    [HttpGet]
    public async Task<List<SidebarViewModel>> GetUserSidebar(string userId)
    {
        var data = await (
            from up in _context.TblUserPermission
            where up.UserId == userId

            join m in _context.TblModule on up.ModuleId equals m.ModuleId
            join me in _context.TblMenu on up.MenuId equals me.MenuId

            select new SidebarViewModel
            {
                ModuleId = m.ModuleId,
                ModuleName = m.ModuleName,
                MenuId = me.MenuId,
                MenuName = me.MenuName,
                ActionId = up.ActionId
            }
        )
        .Distinct()
        .ToListAsync();

        return data;
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            IdentityConstants.ApplicationScheme);

        return RedirectToAction("Login", "Account");
    }
}
