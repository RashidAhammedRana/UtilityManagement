using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
namespace UtilityManagement.Controllers;
public class SectionWiseMonthlyUtilityBillController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SectionWiseMonthlyUtilityBillController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> SectionWiseMonthlyUtilityBillList(
       int page = 1,
       string searchString = "")
    {
        int pageSize = 30;

        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier
        )?.Value;


        // =========================
        // PERMISSION
        // =========================

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Section Wise Monthly Utility Cost")
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


        // =========================
        // CURRENT USER COMPANY
        // =========================

        var currentUserCompany = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();


        ViewBag.CanView = userPermissions.Contains("View");
        ViewBag.CanCreate = userPermissions.Contains("Create");
        ViewBag.CanEdit = userPermissions.Contains("Edit");
        ViewBag.CanDelete = userPermissions.Contains("Delete");


        // =========================
        // BASE QUERY
        // =========================

        var query = _context.TblSectionWiseMonthlyUtilityCost
            .Include(x => x.Com)
            .Include(x => x.Dep)
            .AsQueryable();


        // =========================
        // COMPANY WISE DATA
        // =========================

        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            currentUserCompany = currentUserCompany.Trim();

            query = query.Where(x =>
                x.Com != null &&
                x.Com.ComName == currentUserCompany
            );
        }


        // =========================
        // SEARCH LOGIC
        // =========================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries
            );

            var months = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase)
    {
        { "january", 1 }, { "jan", 1 },
        { "february", 2 }, { "feb", 2 },
        { "march", 3 }, { "mar", 3 },
        { "april", 4 }, { "apr", 4 },
        { "may", 5 },
        { "june", 6 }, { "jun", 6 },
        { "july", 7 }, { "jul", 7 },
        { "august", 8 }, { "aug", 8 },
        { "september", 9 }, { "sep", 9 }, { "sept", 9 },
        { "october", 10 }, { "oct", 10 },
        { "november", 11 }, { "nov", 11 },
        { "december", 12 }, { "dec", 12 }
    };

            // =========================
            // DATE SEARCH
            // =========================

            bool isFullDate =
                DateOnly.TryParseExact(
                    searchString,
                    new[]
                    {
                "dd-MM-yyyy",
                "d-M-yyyy",
                "dd/MM/yyyy",
                "d/M/yyyy",
                "yyyy-MM-dd",
                "yyyy/MM/dd"
                    },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateOnly parsedDate
                );

            if (isFullDate)
            {
                query = query.Where(x =>
                    x.Trdate == parsedDate
                );
            }

            // =========================
            // YEAR-MONTH
            // 2026-06
            // =========================

            else if (parts.Length == 2 && parts[0].Length == 4)
            {
                if (int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month))
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        x.Trdate.Value.Year == year &&
                        x.Trdate.Value.Month == month
                    );
                }
            }

            // =========================
            // MONTH-DAY
            // 06-22 / 22-06
            // =========================

            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b))
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (x.Trdate.Value.Month == a &&
                             x.Trdate.Value.Day == b)
                            ||
                            (x.Trdate.Value.Month == b &&
                             x.Trdate.Value.Day == a)
                        )
                    );
                }
            }

            // =========================
            // MONTH NAME
            // =========================

            else if (months.TryGetValue(
                searchString,
                out int monthNumber))
            {
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value.Month == monthNumber
                );
            }

            // =========================
            // SINGLE NUMBER
            // =========================

            else if (int.TryParse(
                searchString,
                out int number))
            {
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    (
                        x.Trdate.Value.Day == number ||
                        x.Trdate.Value.Month == number ||
                        x.Trdate.Value.Year == number
                    )
                );
            }

            // =========================
            // DEPARTMENT / SECTION
            // =========================

            else
            {
                query = query.Where(x =>
                    x.Dep != null &&
                    x.Dep.DepartmentName.Contains(searchString)
                );
            }
        }



        // =========================
        // PAGINATION
        // =========================

        var totalRecords = await query.CountAsync();


        var monthlyUtilityBillInfo = await query
            .OrderByDescending(x => x.Trdate)
            .ThenByDescending(x => x.Trid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();


        // =========================
        // VIEWBAG
        // =========================

        ViewBag.CurrentPage = page;

        ViewBag.TotalPages =
            (int)Math.Ceiling(
                totalRecords / (double)pageSize
            );

        ViewBag.totalReading = totalRecords;

        ViewBag.SearchString = searchString;


        return View(monthlyUtilityBillInfo);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var userId = _userManager.GetUserId(User);

        var companyName = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        var company = _context.TblCompanyInfo
            .FirstOrDefault(x => x.ComName == companyName);

        if (company == null)
        {
            TempData["ErrorMessage"] = "Company information not found.";
            return RedirectToAction(nameof(SectionWiseMonthlyUtilityBillList));
        }

        ViewBag.CompanyName = company.ComName;

        // All Department / Section
        ViewBag.Departments = _context.TblDepartmentInfo
            .OrderBy(x => x.DepartmentName)
            .ToList();

        var model = new TblSectionWiseMonthlyUtilityCost
        {
            Trdate = DateOnly.FromDateTime(DateTime.Today),
            Comid = company.Comid
        };

        return View(model);
    }




    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblSectionWiseMonthlyUtilityCost sectionWiseMonthlyUtilityCost)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Please fill all required fields.";

                return View(sectionWiseMonthlyUtilityCost);
            }

            // ==========================================
            // CALCULATE TOTAL AMOUNT
            // ==========================================

            sectionWiseMonthlyUtilityCost.TotalCost =
                (sectionWiseMonthlyUtilityCost.ElectricityCost ?? 0)
                + (sectionWiseMonthlyUtilityCost.SteamCost ?? 0)
                + (sectionWiseMonthlyUtilityCost.EtpCost ?? 0)
                + (sectionWiseMonthlyUtilityCost.WtpCost ?? 0)
                + (sectionWiseMonthlyUtilityCost.AcCost ?? 0);

            // ==========================================
            // CREATED INFORMATION
            // ==========================================

            sectionWiseMonthlyUtilityCost.CreatedAt = DateTime.Now;
            sectionWiseMonthlyUtilityCost.CreatedBy =
                User.Identity?.Name ?? "System";

            // ==========================================
            // SAVE
            // ==========================================

            _context.TblSectionWiseMonthlyUtilityCost.Add(sectionWiseMonthlyUtilityCost);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Section Wise Monthly Utility Cost created successfully.";

            return RedirectToAction(
                nameof(SectionWiseMonthlyUtilityBillList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] =
                "Failed to create Section Wise Monthly Utility Cost.";

            return View(sectionWiseMonthlyUtilityCost);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var reading = await _context.TblSectionWiseMonthlyUtilityCost
            .Include(x => x.Com)
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();

        // Company Name
        ViewBag.CompanyName = reading.Com?.ComName;

        // All Departments / Sections
        ViewBag.Departments = await _context.TblDepartmentInfo
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();

        return View(reading);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        TblSectionWiseMonthlyUtilityCost sectionWiseMonthlyUtilityCost)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await LoadEditViewData(sectionWiseMonthlyUtilityCost.Comid);

                return View(sectionWiseMonthlyUtilityCost);
            }

            var existing =
                await _context.TblSectionWiseMonthlyUtilityCost
                    .FirstOrDefaultAsync(
                        x => x.Trid == sectionWiseMonthlyUtilityCost.Trid);

            if (existing == null)
                return NotFound();


            // =====================================================
            // UPDATE BASIC INFORMATION
            // =====================================================

            existing.Trdate = sectionWiseMonthlyUtilityCost.Trdate;
            existing.Depid = sectionWiseMonthlyUtilityCost.Depid;
            existing.Year = sectionWiseMonthlyUtilityCost.Year;
            existing.Month = sectionWiseMonthlyUtilityCost.Month;


            // =====================================================
            // UPDATE UTILITY COST
            // =====================================================

            existing.ElectricityCost =
                sectionWiseMonthlyUtilityCost.ElectricityCost;

            existing.SteamCost =
                sectionWiseMonthlyUtilityCost.SteamCost;

            existing.EtpCost =
                sectionWiseMonthlyUtilityCost.EtpCost;

            existing.WtpCost =
                sectionWiseMonthlyUtilityCost.WtpCost;

            existing.AcCost =
                sectionWiseMonthlyUtilityCost.AcCost;


            // =====================================================
            // PRODUCTION
            // =====================================================

            existing.Production =
                sectionWiseMonthlyUtilityCost.Production;


            // =====================================================
            // CALCULATE TOTAL COST
            // =====================================================

            existing.TotalCost =
                (existing.ElectricityCost ?? 0)
                + (existing.SteamCost ?? 0)
                + (existing.EtpCost ?? 0)
                + (existing.WtpCost ?? 0)
                + (existing.AcCost ?? 0);


            // =====================================================
            // CALCULATE PER UNIT PRODUCTION COST
            // =====================================================

            if (existing.Production.HasValue &&
                existing.Production.Value > 0)
            {
                existing.PerUnitProductionCost =
                    existing.TotalCost.Value /
                    existing.Production.Value;
            }
            else
            {
                existing.PerUnitProductionCost = 0;
            }


            // =====================================================
            // AUDIT
            // =====================================================

            existing.UpdatedAt = DateTime.Now;

            existing.UpdatedBy =
                User.Identity?.Name ?? "System";


            // =====================================================
            // SAVE
            // =====================================================

            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] =
                "Section Wise Monthly Utility Cost updated successfully.";

            return RedirectToAction(
                nameof(SectionWiseMonthlyUtilityBillList));
        }
        catch (Exception)
        {
            await LoadEditViewData(
                sectionWiseMonthlyUtilityCost.Comid);

            TempData["ErrorMessage"] =
                "Failed to update Section Wise Monthly Utility Cost.";

            return View(sectionWiseMonthlyUtilityCost);
        }
    }


    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblSectionWiseMonthlyUtilityCost.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblSectionWiseMonthlyUtilityCost
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(SectionWiseMonthlyUtilityBillList));
        }

        _context.TblSectionWiseMonthlyUtilityCost.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(SectionWiseMonthlyUtilityBillList));
    }

    private async Task LoadEditViewData(int? comid)
    {
        var company = await _context.TblCompanyInfo
            .FirstOrDefaultAsync(x => x.Comid == comid);

        ViewBag.CompanyName = company?.ComName;

        ViewBag.Departments = await _context.TblDepartmentInfo
            .OrderBy(x => x.DepartmentName)
            .ToListAsync();
    }

}




