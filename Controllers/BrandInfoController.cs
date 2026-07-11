using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class BrandInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public BrandInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> BrandInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Brand Information")
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

        var query = _context.TblBrandInfo
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.BrndName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                x.Remarks.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.BrndName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalBrands = totalRecords;
        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblBrandInfo brandInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(brandInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblBrandInfo
                .AnyAsync(x => x.BrndName == brandInfo.BrndName);

            if (isExist)
            {
                ModelState.AddModelError("BrndName", "Brand already exists!");
                return View(brandInfo);
            }

            _context.TblBrandInfo.Add(brandInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Brand Info Created successfully.";

            return RedirectToAction(nameof(BrandInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create Brand.";
            return View(brandInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var brandInfo = await _context.TblBrandInfo.FindAsync(id);
        if (brandInfo == null)
        {
            return NotFound();
        }
        return View(brandInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblBrandInfo brandInfo)
    {
        if (!ModelState.IsValid)
            return View(brandInfo);

        var data = await _context.TblBrandInfo
            .FirstOrDefaultAsync(x => x.Brndid == brandInfo.Brndid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Brand not found.";
            return RedirectToAction(nameof(BrandInfoList));
        }

        data.BrndName = brandInfo.BrndName;
        data.Status = brandInfo.Status;
        data.Remarks = brandInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Brand Info Updated successfully.";

        return RedirectToAction(nameof(BrandInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblBrandInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblBrandInfo
            .FirstOrDefaultAsync(x => x.Brndid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Brand Info not found.";
            return RedirectToAction(nameof(BrandInfoList));
        }

        _context.TblBrandInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Brand Info deleted successfully.";

        return RedirectToAction(nameof(BrandInfoList));
    }

}



