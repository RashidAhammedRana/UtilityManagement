using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

namespace UtilityManagement.Controllers
{
    public class PermissionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermissionController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? userId, int? moduleId, int? menuId)
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            ViewBag.Modules = await _context.TblModule.ToListAsync();
            ViewBag.Menus = await _context.TblMenu.ToListAsync();

            if (string.IsNullOrEmpty(userId))
                return View(new List<PermissionPageVm>());

            var query = _context.TblModule.AsQueryable();

            var permissions = await (
                from m in _context.TblModule
                from me in _context.TblMenu.Where(x => x.ModuleId == m.ModuleId)
                from a in _context.TblPermissionAction

                join up in _context.TblUserPermission
                    .Where(x => x.UserId == userId)
                on new { me.MenuId, a.ActionId }
                equals new { up.MenuId, up.ActionId }
                into temp

                from pg in temp.DefaultIfEmpty()

                where (!moduleId.HasValue || m.ModuleId == moduleId)
                   && (!menuId.HasValue || me.MenuId == menuId)

                select new PermissionPageVm
                {
                    UserId = userId,
                    ModuleId = m.ModuleId,
                    ModuleName = m.ModuleName,
                    MenuId = me.MenuId,
                    MenuName = me.MenuName,
                    ActionId = a.ActionId,
                    ActionName = a.ActionName,
                    IsAllowed = pg != null && pg.IsAllowed
                }
            ).ToListAsync();

            ViewBag.SelectedUserId = userId;

            return View(permissions);
        }


        [HttpPost]
        public async Task<IActionResult> SaveData(string userId, List<PermissionPageVm> model)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index");

            foreach (var item in model)
            {
                var existing = await _context.TblUserPermission
                    .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.ModuleId == item.ModuleId &&
                        x.MenuId == item.MenuId &&
                        x.ActionId == item.ActionId);

                if (item.IsAllowed)
                {
                    if (existing != null)
                    {
                        existing.IsAllowed = true;
                    }
                    else
                    {
                        _context.TblUserPermission.Add(new TblUserPermission
                        {
                            UserId = userId,
                            ModuleId = item.ModuleId,
                            MenuId = item.MenuId,
                            ActionId = item.ActionId,
                            IsAllowed = true
                        });
                    }
                }
                else
                {
                    // যদি unchecked হয় → remove only that row
                    if (existing != null)
                    {
                        _context.TblUserPermission.Remove(existing);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["msg"] = "Permissions Saved";

            return RedirectToAction("Index", new { userId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Index");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            var permissions = await _context.TblUserPermission
                .Where(x => x.UserId == userId)
                .Include(x => x.Menu)
                .Include(x => x.Module)
                .Include(x => x.Action)
                .ToListAsync();

            ViewBag.User = user;

            return View(permissions);
        }
    }
}