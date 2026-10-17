using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;
using UtilityManagement.ViewModels;

public class BiomassStockConsumptionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public BiomassStockConsumptionController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> BiomassStockConsumptionList(int page = 1, string searchString = "")
    {
        int pageSize = 30;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Fuel Stock & Cons.")
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

        var currentUserCompany = await _context.Users.Where(x => x.Id == userId).Select(x => x.Company).FirstOrDefaultAsync();
        // =========================
        // BASE QUERY
        // =========================
        var query = _context.TblBiomassStockConsumptions
            .AsQueryable();
        //Company Wise Data
        if (!string.IsNullOrWhiteSpace(currentUserCompany))
        {
            query = query.Where(x =>
                x.Company != null &&
                x.Company == currentUserCompany);
        }

        // =========================================================
        // SEARCH LOGIC
        // =========================================================

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split(
                '-',
                StringSplitOptions.RemoveEmptyEntries
            );

            bool isNumber =
                int.TryParse(searchString, out int number);


            // =====================================================
            // CASE 1: FULL DATE
            //
            // 09-07-2026  -> 09 July 2026
            // 9-7-2026    -> 09 July 2026
            // 09/07/2026  -> 09 July 2026
            //
            // Also supports:
            // 2026-07-09
            // 2026/07/09
            // =====================================================

            bool isFullDate =
                DateTime.TryParseExact(
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

                    out DateTime parsedDate
                );


            if (isFullDate)
            {
                // Convert DateTime to DateOnly
                var searchDate =
                    DateOnly.FromDateTime(parsedDate);


                // EXACT DATE MATCH
                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value == searchDate
                );
            }


            // =====================================================
            // CASE 2: YEAR-MONTH
            //
            // 2026-07
            // 2026/07
            // =====================================================

            else if (
                parts.Length == 2 &&
                parts[0].Length == 4
            )
            {
                if (
                    int.TryParse(parts[0], out int year) &&
                    int.TryParse(parts[1], out int month)
                )
                {
                    if (month >= 1 && month <= 12)
                    {
                        query = query.Where(x =>
                            x.Trdate.HasValue &&
                            x.Trdate.Value.Year == year &&
                            x.Trdate.Value.Month == month
                        );
                    }
                }
            }


            // =====================================================
            // CASE 3: MONTH-DAY / DAY-MONTH
            //
            // 07-09
            // 09-07
            //
            // Both will find:
            // 09 July
            // =====================================================

            else if (parts.Length == 2)
            {
                if (
                    int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b)
                )
                {
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (
                                x.Trdate.Value.Month == a &&
                                x.Trdate.Value.Day == b
                            )
                            ||
                            (
                                x.Trdate.Value.Month == b &&
                                x.Trdate.Value.Day == a
                            )
                        )
                    );
                }
            }


            // =====================================================
            // CASE 4: SINGLE NUMBER
            //
            // 9
            // 7
            // 2026
            // =====================================================

            else if (isNumber)
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
        }


        // =========================
        // PAGINATION
        // =========================
        var totalRecords = await query.CountAsync();

        var rebReadings = await query
            .OrderByDescending(x => x.Trdate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // =========================
        // VIEWBAG
        // =========================
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.totalReadings = totalRecords;
        ViewBag.SearchString = searchString;

        return View(rebReadings);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadCompanyList();
        LoadRate();
        LoadStock();

        var userId = _userManager.GetUserId(User);

        var currentLocation = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        var lastTrDate = _context.TblDailyStockConsumptions
            .Where(x => x.Company == currentLocation)
            .OrderByDescending(x => x.Trdate)
            .Select(x => x.Trdate)
            .FirstOrDefault();

        var model = new TblBiomassStockConsumption
        {
            Trdate = lastTrDate.HasValue
                ? lastTrDate.Value.AddDays(1)
                : DateOnly.FromDateTime(DateTime.Today),

            SewingJhutOpening = ViewBag.SewingJhutOpeningStock,

            DenimJhutOpening = ViewBag.DenimJhutOpeningStock,
            KnitJhutOpening = ViewBag.KnitJhutOpeningStock,

            CharcoalOpening = ViewBag.CharcoalOpeningStock,

            CartonOpening = ViewBag.CartonOpeningStock,

            PpOpening = ViewBag.PpOpeningStock
        };

        return View(model);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblBiomassStockConsumption biomassStockConsumption)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(biomassStockConsumption);
            }

            // Normalize date (only date part)
            var dateOnly = biomassStockConsumption.Trdate;

            // Save current time (keep datetime but same date)
            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";
            // Created Information
            biomassStockConsumption.CreatedAt = now;
            biomassStockConsumption.CreatedBy = currentUser;

            _context.TblBiomassStockConsumptions.Add(biomassStockConsumption);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading created successfully.";

            return RedirectToAction(nameof(BiomassStockConsumptionList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create reading.";
            return View(biomassStockConsumption);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        LoadCompanyList();
        LoadRate();
        LoadStock();

        var reading = await _context.TblBiomassStockConsumptions
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (reading == null)
            return NotFound();

        reading.SewingJhutOpening = ViewBag.SewingJhutOpeningStock;
        reading.KnitJhutOpening = ViewBag.KnitJhutOpeningStock;
        reading.DenimJhutOpening = ViewBag.DenimJhutOpeningStock;
        reading.CharcoalOpening = ViewBag.CharcoalOpeningStock;
        reading.CartonOpening = ViewBag.CartonOpeningStock;
        reading.PpOpening = ViewBag.PpOpeningStock;
        return View(reading);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblBiomassStockConsumption biomassStockConsumption)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(biomassStockConsumption);
            }

            var existing = await _context.TblBiomassStockConsumptions
                .FirstOrDefaultAsync(x => x.Trid == biomassStockConsumption.Trid);

            if (existing == null)
            {
                return NotFound();
            }

            // Update editable fields
            existing.Trdate = biomassStockConsumption.Trdate;
            existing.Company = biomassStockConsumption.Company;

            existing.SewingJhutOpening = biomassStockConsumption.SewingJhutOpening;
            existing.SewingJhutReceive = biomassStockConsumption.SewingJhutReceive;
            existing.SewingJhutB1Cons = biomassStockConsumption.SewingJhutB1Cons;
            existing.SewingJhutB2Cons = biomassStockConsumption.SewingJhutB2Cons;
            existing.SewingJhutTotalCons = biomassStockConsumption.SewingJhutTotalCons;
            existing.SewingJhutTotalCost = biomassStockConsumption.SewingJhutTotalCost;
            existing.SewingJhutClosing = biomassStockConsumption.SewingJhutClosing;

            existing.KnitJhutOpening = biomassStockConsumption.KnitJhutOpening;
            existing.KnitJhutReceive = biomassStockConsumption.KnitJhutReceive;
            existing.KnitJhutB1Cons = biomassStockConsumption.KnitJhutB1Cons;
            existing.KnitJhutB2Cons = biomassStockConsumption.KnitJhutB2Cons;
            existing.KnitJhutTotalCons = biomassStockConsumption.KnitJhutTotalCons;
            existing.KnitJhutTotalCost = biomassStockConsumption.KnitJhutTotalCost;
            existing.KnitJhutClosing = biomassStockConsumption.KnitJhutClosing;

            existing.DenimJhutOpening = biomassStockConsumption.DenimJhutOpening;
            existing.DenimJhutReceive = biomassStockConsumption.DenimJhutReceive;
            existing.DenimJhutB1Cons = biomassStockConsumption.DenimJhutB1Cons;
            existing.DenimJhutB2Cons = biomassStockConsumption.DenimJhutB2Cons;
            existing.DenimJhutTotalCons = biomassStockConsumption.DenimJhutTotalCons;
            existing.DenimJhutTotalCost = biomassStockConsumption.DenimJhutTotalCost;
            existing.DenimJhutClosing = biomassStockConsumption.DenimJhutClosing;

            existing.CharcoalOpening = biomassStockConsumption.CharcoalOpening;
            existing.CharcoalReceive = biomassStockConsumption.CharcoalReceive;
            existing.CharcoalB1Cons = biomassStockConsumption.CharcoalB1Cons;
            existing.CharcoalB2Cons = biomassStockConsumption.CharcoalB2Cons;
            existing.CharcoalTotalCons = biomassStockConsumption.CharcoalTotalCons;
            existing.CharcoalTotalCost = biomassStockConsumption.CharcoalTotalCost;
            existing.CharcoalClosing = biomassStockConsumption.CharcoalClosing;

            existing.PpOpening = biomassStockConsumption.PpOpening;
            existing.PpReceive = biomassStockConsumption.PpReceive;
            existing.PpB1Cons = biomassStockConsumption.PpB1Cons;
            existing.PpB2Cons = biomassStockConsumption.PpB2Cons;
            existing.PpTotalCons = biomassStockConsumption.PpTotalCons;
            existing.PpTotalCost = biomassStockConsumption.PpTotalCost;
            existing.PpClosing = biomassStockConsumption.PpClosing;

            existing.CartonOpening = biomassStockConsumption.CartonOpening;
            existing.CartonReceive = biomassStockConsumption.CartonReceive;
            existing.CartonB1Cons = biomassStockConsumption.CartonB1Cons;
            existing.CartonB2Cons = biomassStockConsumption.CartonB2Cons;
            existing.CartonTotalCons = biomassStockConsumption.CartonTotalCons;
            existing.CartonTotalCost = biomassStockConsumption.CartonTotalCost;
            existing.CartonClosing = biomassStockConsumption.CartonClosing;

            // Update audit fields
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = User.Identity?.Name ?? "System";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reading updated successfully.";

            return RedirectToAction(nameof(BiomassStockConsumptionList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update reading.";

            return View(biomassStockConsumption);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblBiomassStockConsumptions.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblBiomassStockConsumptions
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Readings not found.";
            return RedirectToAction(nameof(BiomassStockConsumptionList));
        }

        _context.TblBiomassStockConsumptions.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Readings deleted successfully.";

        return RedirectToAction(nameof(BiomassStockConsumptionList));
    }
    [HttpGet]
    private void LoadCompanyList()
    {
        var userId = _userManager.GetUserId(User);

        var currentCompany = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        ViewBag.CurrentCompany = currentCompany;
    }
    private void LoadRate()
    {
        ViewBag.SewingBiomassRate = _context.TblFncItemRates
            .Where(x => x.Fncid == 32)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();

        ViewBag.DenimBiomassRate = _context.TblFncItemRates
            .Where(x => x.Fncid == 37)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();

        ViewBag.CartonRate = _context.TblFncItemRates
            .Where(x => x.Fncid == 35)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();

        ViewBag.CharcoalRate = _context.TblFncItemRates
            .Where(x => x.Fncid == 36)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();

        ViewBag.PpRate = _context.TblFncItemRates
            .Where(x => x.Fncid == 38)
            .OrderByDescending(x => x.Date)
            .Select(x => x.Rate)
            .FirstOrDefault();
    }
    private void LoadStock()
    {
        var userId = _userManager.GetUserId(User);
        var currentLocation = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();

        var latestStock = _context.TblBiomassStockConsumptions
            .Where(x => x.Company == currentLocation)
            .OrderByDescending(x => x.Trdate)
            .ThenByDescending(x => x.Trid)
            .FirstOrDefault();

        var sewingJhutOpeningStock = latestStock?.SewingJhutClosing ?? 0;
        var denimJhutOpeningStock = latestStock?.DenimJhutClosing ?? 0;
        var knitJhutOpeningStock = latestStock?.KnitJhutClosing ?? 0;
        var charCoalOpeningStock = latestStock?.CharcoalClosing ?? 0;
        var cartonOpeningStock = latestStock?.CartonClosing ?? 0;
        var ppOpeningStock = latestStock?.PpClosing ?? 0;

        ViewBag.SewingJhutOpeningStock = sewingJhutOpeningStock;
        ViewBag.DenimJhutOpeningStock = denimJhutOpeningStock;
        ViewBag.KnitJhutOpeningStock = knitJhutOpeningStock;
        ViewBag.CharcoalOpeningStock = charCoalOpeningStock;
        ViewBag.CartonOpeningStock = cartonOpeningStock;
        ViewBag.PpOpeningStock = ppOpeningStock;
    }
}


