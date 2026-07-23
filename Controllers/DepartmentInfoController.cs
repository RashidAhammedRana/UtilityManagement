using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class DepartmentInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public DepartmentInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DepartmentInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Department Information")
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

        var query = _context.TblDepartmentInfo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();
            query = query.Where(x =>
                x.DepartmentName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                (x.Remarks != null && x.Remarks.Contains(searchString))
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.DepartmentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalDepartments = totalRecords;

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblDepartmentInfo departmentInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(departmentInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblDepartmentInfo
                .AnyAsync(x => x.DepartmentName == departmentInfo.DepartmentName);

            if (isExist)
            {
                ModelState.AddModelError("DepartmentName", "Department already exists!");
                return View(departmentInfo);
            }

            _context.TblDepartmentInfo.Add(departmentInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Department Info Created successfully.";

            return RedirectToAction(nameof(DepartmentInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(departmentInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var departmentinfo = await _context.TblDepartmentInfo.FindAsync(id);
        if (departmentinfo == null)
        {
            return NotFound();
        }
        return View(departmentinfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblDepartmentInfo departmentInfo)
    {
        if (!ModelState.IsValid)
            return View(departmentInfo);

        var data = await _context.TblDepartmentInfo
            .FirstOrDefaultAsync(x => x.Depid == departmentInfo.Depid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Department not found.";
            return RedirectToAction(nameof(DepartmentInfoList));
        }

        data.DepartmentName = departmentInfo.DepartmentName;
        data.Status = departmentInfo.Status;
        data.Remarks = departmentInfo.Remarks;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Department Info Updated successfully.";

        return RedirectToAction(nameof(DepartmentInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblDepartmentInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblDepartmentInfo
            .FirstOrDefaultAsync(x => x.Depid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Department Info not found.";
            return RedirectToAction(nameof(DepartmentInfoList));
        }

        _context.TblDepartmentInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Department Info deleted successfully.";

        return RedirectToAction(nameof(DepartmentInfoList));
    }
}
