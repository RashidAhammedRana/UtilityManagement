using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class BuildingInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public BuildingInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> BuildingInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Building Information")
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

        var query = _context.TblBuildingInfo
            .Include(x => x.Com)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.BldName.Contains(searchString) ||
                x.Com.ComName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                x.Remarks.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.BldName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalBuildings = totalRecords;

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblBuildingInfo();

        ViewBag.CompanyList = _context.TblCompanyInfo
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
    public async Task<IActionResult> Create(TblBuildingInfo buildingInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(buildingInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            //var isExist = await _context.TblBuildingInfo
            //    .AnyAsync(x => x.BldName == buildingInfo.BldName);

            //if (isExist)
            //{
            //    ModelState.AddModelError("BldName", "Building already exists!");
            //    return View(buildingInfo);
            //}

            _context.TblBuildingInfo.Add(buildingInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Building Info Created successfully.";

            return RedirectToAction(nameof(BuildingInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(buildingInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var buildingInfo = await _context.TblBuildingInfo.FindAsync(id);
        if (buildingInfo == null)
        {
            return NotFound();
        }

        ViewBag.CompanyList = _context.TblCompanyInfo
            .Select(x => new SelectListItem
            {
                Value = x.Comid.ToString(),
                Text = x.ComName
            })
            .ToList();
        return View(buildingInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblBuildingInfo buildingInfo)
    {
        if (!ModelState.IsValid)
            return View(buildingInfo);

        var data = await _context.TblBuildingInfo
            .FirstOrDefaultAsync(x => x.Comid == buildingInfo.Bldid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Building not found.";
            return RedirectToAction(nameof(BuildingInfoList));
        }

        data.Comid = buildingInfo.Comid;
        data.BldName = buildingInfo.BldName;
        data.Status = buildingInfo.Status;
        data.Remarks = buildingInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Building Info Updated successfully.";

        return RedirectToAction(nameof(BuildingInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblBuildingInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int bldId)
    {
        var data = await _context.TblBuildingInfo
            .FirstOrDefaultAsync(x => x.Bldid == bldId);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Building Info not found.";
            return RedirectToAction(nameof(BuildingInfoList));
        }

        _context.TblBuildingInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Building Info deleted successfully.";

        return RedirectToAction(nameof(BuildingInfoList));
    }
}

