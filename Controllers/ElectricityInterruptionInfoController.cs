using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class ElectricityInterruptionInfoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ElectricityInterruptionInfoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ElectricityInterruptionInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Elec. Intr. Reading")
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

        var query = _context.TblElectricityInterruptionInfo
            .Include(x => x.Company)
            .Include(x => x.Department)
            .Include(x => x.InterruptionType)
            .Include(x => x.Reason)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();
            query = query.Where(x =>
            (x.Company != null && x.Company.ComName!.Contains(searchString)) ||
            (x.Department != null && x.Department.DepartmentName!.Contains(searchString)) ||
            (x.InterruptionType != null && x.InterruptionType.InterruptionTypeName!.Contains(searchString)) ||
            (x.Reason != null && x.Reason.ReasonName!.Contains(searchString))
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.Eiid)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalElectricityInterruptions = totalRecords;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {

        var model = new TblElectricityInterruptionInfo
        {
            Date = DateTime.Today
        };
        ViewBag.InterruptiotypeList = await _context.TblInterruptionTypeInfo
        .OrderBy(x => x.InterruptionTypeName)
        .Select(x => new SelectListItem
        {
            Value = x.Itid.ToString(),
            Text = x.InterruptionTypeName
        })
        .ToListAsync();

        ViewBag.DepartmentList = await _context.TblDepartmentInfo
        .OrderBy(x => x.DepartmentName)
        .Select(x => new SelectListItem
        {
            Value = x.Depid.ToString(),
            Text = x.DepartmentName
        })
        .ToListAsync();

        ViewBag.ReasonList = await _context.TblReasonInfo
        .OrderBy(x => x.ReasonName)
        .Select(x => new SelectListItem
        {
            Value = x.Rid.ToString(),
            Text = x.ReasonName
        })
        .ToListAsync();

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
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblElectricityInterruptionInfo electricityInterruptionInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(electricityInterruptionInfo);
            }
            // Created Information
            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";
            electricityInterruptionInfo.CreatedAt = now;
            electricityInterruptionInfo.CreatedBy = currentUser;
            _context.TblElectricityInterruptionInfo.Add(electricityInterruptionInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Data Created successfully.";

            return RedirectToAction(nameof(ElectricityInterruptionInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(electricityInterruptionInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var electricityInterruptionInfo = await _context.TblElectricityInterruptionInfo
            .FindAsync(id);

        if (electricityInterruptionInfo == null)
        {
            return NotFound();
        }

        ViewBag.InterruptiotypeList = await _context.TblInterruptionTypeInfo
            .OrderBy(x => x.InterruptionTypeName)
            .Select(x => new SelectListItem
            {
                Value = x.Itid.ToString(),
                Text = x.InterruptionTypeName
            })
            .ToListAsync();

        ViewBag.DepartmentList = await _context.TblDepartmentInfo
            .OrderBy(x => x.DepartmentName)
            .Select(x => new SelectListItem
            {
                Value = x.Depid.ToString(),
                Text = x.DepartmentName
            })
            .ToListAsync();

        ViewBag.ReasonList = await _context.TblReasonInfo
            .Where(x => x.Itid == electricityInterruptionInfo.Itid)
            .OrderBy(x => x.ReasonName)
            .Select(x => new SelectListItem
            {
                Value = x.Rid.ToString(),
                Text = x.ReasonName
            })
            .ToListAsync();

        var userId = _userManager.GetUserId(User);

        var userCompany = await _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefaultAsync();

        ViewBag.CompanyList = await _context.TblCompanyInfo
            .Where(x => x.ComName == userCompany)
            .Select(x => new SelectListItem
            {
                Value = x.Comid.ToString(),
                Text = x.ComName
            })
            .ToListAsync();

        return View(electricityInterruptionInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblElectricityInterruptionInfo electricityInterruptionInfo)
    {
        if (!ModelState.IsValid)
            return View(electricityInterruptionInfo);

        var data = await _context.TblElectricityInterruptionInfo
            .FirstOrDefaultAsync(x => x.Eiid == electricityInterruptionInfo.Eiid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(ElectricityInterruptionInfoList));
        }


        data.Date = electricityInterruptionInfo.Date;
        data.Depid = electricityInterruptionInfo.Depid;
        data.PowerOffTime = electricityInterruptionInfo.PowerOffTime;
        data.PowerOnTime = electricityInterruptionInfo.PowerOnTime;
        data.DurationMin = electricityInterruptionInfo.DurationMin;
        data.Itid = electricityInterruptionInfo.Itid;
        data.Rid = electricityInterruptionInfo.Rid;
        data.Remarks = electricityInterruptionInfo.Remarks;

        // Update audit fields
        data.UpdatedAt = DateTime.Now;
        data.UpdatedBy = User.Identity?.Name ?? "System";
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Data Updated successfully.";
        return RedirectToAction(nameof(ElectricityInterruptionInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblElectricityInterruptionInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblElectricityInterruptionInfo
            .FirstOrDefaultAsync(x => x.Eiid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Data not found.";
            return RedirectToAction(nameof(ElectricityInterruptionInfoList));
        }

        _context.TblElectricityInterruptionInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Data deleted successfully.";

        return RedirectToAction(nameof(ElectricityInterruptionInfoList));
    }

    [HttpGet]
    public async Task<JsonResult> GetReasonsByType(int itid)
    {
        var reasons = await _context.TblReasonInfo
            .Where(x => x.Itid == itid)
            .OrderBy(x => x.ReasonName)
            .Select(x => new
            {
                value = x.Rid,
                text = x.ReasonName
            })
            .ToListAsync();

        return Json(reasons);
    }
}


