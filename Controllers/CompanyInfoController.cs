using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class CompanyInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public CompanyInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CompanyInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Company Information")
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

        var query = _context.TblCompanyInfo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.ComName.Contains(searchString) ||
                x.ComEmail.Contains(searchString) ||
                x.ComAddress.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.ComName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalCompanies = totalRecords;

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblCompanyInfo companyInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(companyInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblCompanyInfo
                .AnyAsync(x => x.ComName == companyInfo.ComName);

            if (isExist)
            {
                ModelState.AddModelError("ComName", "Company already exists!");
                return View(companyInfo);
            }

            _context.TblCompanyInfo.Add(companyInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Company Info Created successfully.";

            return RedirectToAction(nameof(CompanyInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(companyInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var companyinfo = await _context.TblCompanyInfo.FindAsync(id);
        if (companyinfo == null)
        {
            return NotFound();
        }
        return View(companyinfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblCompanyInfo companyInfo)
    {
        if (!ModelState.IsValid)
            return View(companyInfo);

        var data = await _context.TblCompanyInfo
            .FirstOrDefaultAsync(x => x.Comid == companyInfo.Comid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Company not found.";
            return RedirectToAction(nameof(CompanyInfoList));
        }

        data.ComName = companyInfo.ComName;
        data.ComContact = companyInfo.ComContact;
        data.ComEmail = companyInfo.ComEmail;
        data.ComAddress = companyInfo.ComAddress;
        data.Status = companyInfo.Status;
        data.Remarks = companyInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Company Info Updated successfully.";

        return RedirectToAction(nameof(CompanyInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblCompanyInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int comId)
    {
        var data = await _context.TblCompanyInfo
            .FirstOrDefaultAsync(x => x.Comid == comId);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Company Info not found.";
            return RedirectToAction(nameof(CompanyInfoList));
        }

        _context.TblCompanyInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Company Info deleted successfully.";

        return RedirectToAction(nameof(CompanyInfoList));
    }
}
