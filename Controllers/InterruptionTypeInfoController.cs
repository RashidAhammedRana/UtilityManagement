using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class InterruptionTypeInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public InterruptionTypeInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> InterruptionTypeInfolist(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Interruption Type Info")
            .Select(x => x.MenuId)
            .FirstOrDefaultAsync();

        var userPermissions = await (
            from up in _context.TblUserPermission
            join pa in _context.TblPermissionAction
                on up.ActionId equals pa.ActionId
            where up.UserId == userId
                  && up.MenuId == menuId
                  && up.IsAllowed
            select pa.ActionName
        ).ToListAsync();

        ViewBag.CanView = userPermissions.Contains("View");
        ViewBag.CanCreate = userPermissions.Contains("Create");
        ViewBag.CanEdit = userPermissions.Contains("Edit");
        ViewBag.CanDelete = userPermissions.Contains("Delete");

        var query = _context.TblInterruptionTypeInfo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();
            query = query.Where(x =>
                x.InterruptionTypeName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                (x.Remarks != null && x.Remarks.Contains(searchString))
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.InterruptionTypeName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totaltypes = totalRecords;

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblInterruptionTypeInfo interruptionTypeInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(interruptionTypeInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblInterruptionTypeInfo
                .AnyAsync(x => x.InterruptionTypeName == interruptionTypeInfo.InterruptionTypeName);

            if (isExist)
            {
                ModelState.AddModelError("InterruptionTypeName", "Interruption Type already exists!");
                return View(interruptionTypeInfo);
            }

            _context.TblInterruptionTypeInfo.Add(interruptionTypeInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Interruption Type Info Created successfully.";

            return RedirectToAction(nameof(InterruptionTypeInfolist));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(interruptionTypeInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var interruptionTypeInfo = await _context.TblInterruptionTypeInfo.FindAsync(id);
        if (interruptionTypeInfo == null)
        {
            return NotFound();
        }
        return View(interruptionTypeInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblInterruptionTypeInfo interruptionTypeInfo)
    {
        if (!ModelState.IsValid)
            return View(interruptionTypeInfo);

        var data = await _context.TblInterruptionTypeInfo
            .FirstOrDefaultAsync(x => x.Itid == interruptionTypeInfo.Itid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Interruption Type not found.";
            return RedirectToAction(nameof(InterruptionTypeInfolist));
        }

        data.InterruptionTypeName = interruptionTypeInfo.InterruptionTypeName;
        data.Status = interruptionTypeInfo.Status;
        data.Remarks = interruptionTypeInfo.Remarks;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Interruption Type Info Updated successfully.";

        return RedirectToAction(nameof(InterruptionTypeInfolist));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblInterruptionTypeInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblInterruptionTypeInfo
            .FirstOrDefaultAsync(x => x.Itid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Interruption Type Info not found.";
            return RedirectToAction(nameof(InterruptionTypeInfolist));
        }

        _context.TblInterruptionTypeInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Interruption Type Info deleted successfully.";

        return RedirectToAction(nameof(InterruptionTypeInfolist));
    }
}
