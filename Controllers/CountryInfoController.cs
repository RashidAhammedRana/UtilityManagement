using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class CountryInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public CountryInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult>CountryInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Country Information")
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

        var query = _context.TblCountryInfo
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.CntName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                x.Remarks.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.CntName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalCountries = totalRecords;
        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblCountryInfo countryInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(countryInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblCountryInfo
                .AnyAsync(x => x.CntName == countryInfo.CntName);

            if (isExist)
            {
                ModelState.AddModelError("CntName", "Country already exists!");
                return View(countryInfo);
            }

            _context.TblCountryInfo.Add(countryInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Country Info Created successfully.";

            return RedirectToAction(nameof(CountryInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create Country.";
            return View(countryInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var countryInfo = await _context.TblCountryInfo.FindAsync(id);
        if (countryInfo == null)
        {
            return NotFound();
        }
        return View(countryInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblCountryInfo countryInfo)
    {
        if (!ModelState.IsValid)
            return View(countryInfo);

        var data = await _context.TblCountryInfo
            .FirstOrDefaultAsync(x => x.Cntid == countryInfo.Cntid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Country not found.";
            return RedirectToAction(nameof(CountryInfoList));
        }

        data.CntCode = countryInfo.CntCode;
        data.CntName = countryInfo.CntName;
        data.Status = countryInfo.Status;
        data.Remarks = countryInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Country Info Updated successfully.";

        return RedirectToAction(nameof(CountryInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblCountryInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblCountryInfo
            .FirstOrDefaultAsync(x => x.Cntid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Country Info not found.";
            return RedirectToAction(nameof(CountryInfoList));
        }

        _context.TblCountryInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Country Info deleted successfully.";

        return RedirectToAction(nameof(CountryInfoList));
    }

}



