using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class LoadChartMasterFileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoadChartMasterFileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager
)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> LoadChartMasterFileList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Electrical Load Chart")
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

        // =========================
        // BASE QUERY
        // =========================
        var query = _context.TblLoadChartMasterFile
            .Include(x => x.Company)
            .Include(x => x.Building)
            .Include(x => x.Fl)
            .Include(x => x.Cnt)
            .Include(x => x.Lt)
            .Include(x => x.Brnd)
            .AsQueryable();

        // =========================
        // SEARCH LOGIC
        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            var parts = searchString.Split('-', StringSplitOptions.RemoveEmptyEntries);

            int number;
            bool isNumber = int.TryParse(searchString, out number);

            // =========================
            // CASE 1: FULL DATE (22-06-2026 OR 2026-06-22 OR 22/06/2026)
            // =========================
            bool isFullDate =
                DateTime.TryParseExact(
                    searchString,
                    new[] {
                "dd-MM-yyyy", "d-M-yyyy",
                "dd/MM/yyyy", "d/M/yyyy",
                "yyyy-MM-dd", "yyyy/MM/dd"
                    },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime parsedDate
                );

            if (isFullDate)
            {
                var start = parsedDate.Date;
                var end = start.AddDays(1);

                query = query.Where(x =>
                    x.Trdate.HasValue &&
                    x.Trdate.Value >= start &&
                    x.Trdate.Value < end
                );
            }

            // =========================
            // CASE 2: YEAR-MONTH (2026-06 / 2026/06)
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
            // CASE 3: MONTH-DAY (06-22 / 22-06)
            // =========================
            else if (parts.Length == 2)
            {
                if (int.TryParse(parts[0], out int a) &&
                    int.TryParse(parts[1], out int b))
                {
                    // assume MM-DD or DD-MM both supported
                    query = query.Where(x =>
                        x.Trdate.HasValue &&
                        (
                            (x.Trdate.Value.Month == a && x.Trdate.Value.Day == b) ||
                            (x.Trdate.Value.Month == b && x.Trdate.Value.Day == a)
                        )
                    );
                }
            }

            // =========================
            // CASE 4: SINGLE NUMBER (day/month/year)
            // =========================
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

        var loadChartFiles = await query
            .OrderByDescending(x => x.Trdate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // =========================
        // VIEWBAG
        // =========================
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.totalFiles = totalRecords;
        ViewBag.SearchString = searchString;

        return View(loadChartFiles);
    }
    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblLoadChartMasterFile
        {
            Trdate = DateTime.Today,
            Pf = 0.90
        };
        var userId = _userManager.GetUserId(User);

        var userCompany = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();


        ViewBag.CompanyList = _context.TblCompanyInfo
            .Where(x => x.ComName == userCompany)
            .Select(x => new SelectListItem
            {
                Value = x.Comid.ToString(),
                Text = x.ComName
            })
            .ToList();

        ViewBag.CountryList = _context.TblCountryInfo
           .Select(x => new SelectListItem
           {
               Value = x.Cntid.ToString(),
               Text = $"{x.CntName}"
           })
           .ToList();
        ViewBag.LoadTypeList = _context.TblLoadTypeInfo
           .Select(x => new SelectListItem
           {
               Value = x.Ltid.ToString(),
               Text = $"{x.Ltname}"
           })
           .ToList();
        ViewBag.BrandList = _context.TblBrandInfo
   .Select(x => new SelectListItem
   {
       Value = x.Brndid.ToString(),
       Text = $"{x.BrndName}"
   })
   .ToList();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblLoadChartMasterFile loadChartMasterFile)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(loadChartMasterFile);
            }

            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";

            loadChartMasterFile.Trdate = now;
            loadChartMasterFile.CreatedAt = now;
            loadChartMasterFile.CreatedBy = currentUser;

            _context.TblLoadChartMasterFile.Add(loadChartMasterFile);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "File created successfully.";

            return RedirectToAction(nameof(LoadChartMasterFileList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create File.";
            return View(loadChartMasterFile);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await _context.TblLoadChartMasterFile
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (model == null)
            return NotFound();

        var userId = _userManager.GetUserId(User);

        var userCompany = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();


        ViewBag.CompanyList = _context.TblCompanyInfo
            .Where(x => x.ComName == userCompany)
            .Select(x => new SelectListItem
            {
                Value = x.Comid.ToString(),
                Text = x.ComName
            })
            .ToList();

        ViewBag.BuidingList = _context.TblBuildingInfo
            .Where(x => x.Comid == model.Comid)
            .Select(x => new SelectListItem
            {
                Value = x.Bldid.ToString(),
                Text = x.BldName
            }).ToList();

        ViewBag.FloorList = _context.TblFloorInfo
            .Where(x => x.Comid == model.Comid &&
                        x.Bldid == model.Bldid)
            .Select(x => new SelectListItem
            {
                Value = x.Flid.ToString(),
                Text = x.Flname
            }).ToList();

        ViewBag.LoadTypeList = _context.TblLoadTypeInfo
            .Select(x => new SelectListItem
            {
                Value = x.Ltid.ToString(),
                Text = x.Ltname
            }).ToList();
        ViewBag.CountryList = _context.TblCountryInfo
            .Select(x => new SelectListItem
            {
                Value = x.Cntid.ToString(),
                Text = $"{x.CntName}"
            })
            .ToList();
        ViewBag.LoadTypeList = _context.TblLoadTypeInfo
           .Select(x => new SelectListItem
           {
               Value = x.Ltid.ToString(),
               Text = $"{x.Ltname}"
           })
           .ToList();
        ViewBag.BrandList = _context.TblBrandInfo
            .Select(x => new SelectListItem
            {
                Value = x.Brndid.ToString(),
                Text = $"{x.BrndName}"
            })
            .ToList();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblLoadChartMasterFile loadChartMasterFile)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(loadChartMasterFile);
            }

            //_context.Update(loadChartMasterFile);
            var existing = await _context.TblLoadChartMasterFile
             .FirstOrDefaultAsync(x => x.Trid == loadChartMasterFile.Trid);

            if (existing == null)
            {
                return NotFound();
            }
            // Update editable fields
            existing.Comid = loadChartMasterFile.Comid;
            existing.Bldid = loadChartMasterFile.Bldid;
            existing.Flid = loadChartMasterFile.Flid;
            existing.Ltid = loadChartMasterFile.Ltid;
            existing.Description = loadChartMasterFile.Description;
            existing.Type = loadChartMasterFile.Type;
            existing.Cntid = loadChartMasterFile.Cntid;
            existing.Brndid = loadChartMasterFile.Brndid;
            existing.Model = loadChartMasterFile.Model;
            existing.SlNo = loadChartMasterFile.SlNo;
            existing.Capacity = loadChartMasterFile.Capacity;
            existing.CmsnDate = loadChartMasterFile.CmsnDate;
            existing.Watt = loadChartMasterFile.Watt;
            existing.Qty = loadChartMasterFile.Qty;
            existing.SubTotalWatt = loadChartMasterFile.SubTotalWatt;
            existing.Volt = loadChartMasterFile.Volt;
            existing.Pf = loadChartMasterFile.Pf;
            existing.AmpSignalPhase = loadChartMasterFile.AmpSignalPhase;
            existing.AmpThreePhase = loadChartMasterFile.AmpThreePhase;
            existing.SubTotalKw400v = loadChartMasterFile.SubTotalKw400v;
            existing.StandbyLoadKw = loadChartMasterFile.StandbyLoadKw;
            existing.TotalLoadWithoutStandby = loadChartMasterFile.TotalLoadWithoutStandby;
            existing.Remarks = loadChartMasterFile.Remarks;
            // Update audit fields
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = User.Identity?.Name ?? "System";
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "File updated successfully.";

            return RedirectToAction(nameof(LoadChartMasterFileList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to update File.";
            return View(loadChartMasterFile);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblLoadChartMasterFile.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblLoadChartMasterFile
            .FirstOrDefaultAsync(x => x.Trid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "File not found.";
            return RedirectToAction(nameof(LoadChartMasterFileList));
        }

        _context.TblLoadChartMasterFile.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "File deleted successfully.";

        return RedirectToAction(nameof(LoadChartMasterFileList));
    }

    [HttpGet]
    public JsonResult GetBuildingByCompany(int companyId)
    {
        var data = _context.TblBuildingInfo
            .Where(x => x.Comid == companyId)
            .Select(x => new
            {
                value = x.Bldid,
                text = x.BldName
            })
            .ToList();

        return Json(data);
    }

    [HttpGet]
    public JsonResult GetFloorByBuilding(int companyId, int buildingId)
    {
        var data = _context.TblFloorInfo
            .Where(x => x.Comid == companyId &&
                        x.Bldid == buildingId)
            .Select(x => new
            {
                value = x.Flid,
                text = x.Flname
            })
            .ToList();

        return Json(data);
    }
}



