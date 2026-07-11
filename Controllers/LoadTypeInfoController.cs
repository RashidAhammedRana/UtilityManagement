using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class LoadTypeInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public LoadTypeInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> LoadTypeInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Load Type")
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

        var query = _context.TblLoadTypeInfo
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.Ltname.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                x.Remarks.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.Ltname)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalTypes = totalRecords;
        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblLoadTypeInfo loadTypeInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(loadTypeInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblLoadTypeInfo
                .AnyAsync(x => x.Ltname == loadTypeInfo.Ltname);

            if (isExist)
            {
                ModelState.AddModelError("Ltname", "Load Type already exists!");
                return View(loadTypeInfo);
            }

            _context.TblLoadTypeInfo.Add(loadTypeInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Load Type  Info Created successfully.";

            return RedirectToAction(nameof(LoadTypeInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create Load Type.";
            return View(loadTypeInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var loadTypeInfo = await _context.TblLoadTypeInfo.FindAsync(id);
        if (loadTypeInfo == null)
        {
            return NotFound();
        }
        return View(loadTypeInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblLoadTypeInfo loadTypeInfo)
    {
        if (!ModelState.IsValid)
            return View(loadTypeInfo);

        var data = await _context.TblLoadTypeInfo
            .FirstOrDefaultAsync(x => x.Ltid == loadTypeInfo.Ltid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Load Type  not found.";
            return RedirectToAction(nameof(LoadTypeInfoList));
        }

        data.Ltname = loadTypeInfo.Ltname;
        data.Status = loadTypeInfo.Status;
        data.Remarks = loadTypeInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Load Type Info Updated successfully.";

        return RedirectToAction(nameof(LoadTypeInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblLoadTypeInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblLoadTypeInfo
            .FirstOrDefaultAsync(x => x.Ltid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Load Type Info not found.";
            return RedirectToAction(nameof(LoadTypeInfoList));
        }

        _context.TblLoadTypeInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Load Type Info deleted successfully.";

        return RedirectToAction(nameof(LoadTypeInfoList));
    }

}




