using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UtilityManagement.Data;
using UtilityManagement.Models;

public class FloorInfoController : Controller
{
    private readonly ApplicationDbContext _context;

    public FloorInfoController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> FloorInfoList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Floor Information")
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

        //var query = _context.TblFloorInfo.AsQueryable();
        var query = _context.TblFloorInfo
                .Include(x => x.Company)
                .Include(x => x.Building)
                .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.Flname.Contains(searchString) ||
                x.Company.ComName.Contains(searchString) ||
                x.Building.BldName.Contains(searchString) ||
                x.Status.Contains(searchString) ||
                x.Remarks.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.Flname)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalFloors = totalRecords;

        return View(data);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new TblFloorInfo();

        ViewBag.CompanyList = _context.TblCompanyInfo
            .Select(x => new SelectListItem
            {
                Value = x.Comid.ToString(),
                Text = x.ComName
            })
            .ToList();
        //ViewBag.BuildingList = _context.TblBuildingInfo
        //    .Select(x => new SelectListItem
        //    {
        //        Value = x.Bldid.ToString(),
        //        Text = x.BldName
        //    })
        //    .ToList();
        return View(model);
    }

    [HttpGet]
    public JsonResult GetBuildings(int companyId)
    {
        var buildings = _context.TblBuildingInfo
            .Where(x => x.Comid == companyId)
            .Select(x => new
            {
                value = x.Bldid,
                text = x.BldName
            })
            .ToList();

        return Json(buildings);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblFloorInfo floorInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(floorInfo);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblFloorInfo
                .AnyAsync(x => x.Flname == floorInfo.Flname);

            if (isExist)
            {
                ModelState.AddModelError("Flname", "Floor already exists!");
                return View(floorInfo);
            }

            _context.TblFloorInfo.Add(floorInfo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Floor Info Created successfully.";

            return RedirectToAction(nameof(FloorInfoList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create.";
            return View(floorInfo);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var floorInfo = await _context.TblFloorInfo.FindAsync(id);
        if (floorInfo == null)
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
        ViewBag.BuildingList = _context.TblBuildingInfo
            .Select(x => new SelectListItem
            {
                Value = x.Bldid.ToString(),
                Text = x.BldName
            })
            .ToList();
        return View(floorInfo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblFloorInfo floorInfo)
    {
        if (!ModelState.IsValid)
            return View(floorInfo);

        var data = await _context.TblFloorInfo
            .FirstOrDefaultAsync(x => x.Flid == floorInfo.Flid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Floor not found.";
            return RedirectToAction(nameof(FloorInfoList));
        }

        data.Comid = floorInfo.Comid;
        data.Bldid = floorInfo.Bldid;
        data.Flname = floorInfo.Flname;
        data.Status = floorInfo.Status;
        data.Remarks = floorInfo.Remarks;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Floor Info Updated successfully.";

        return RedirectToAction(nameof(FloorInfoList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblFloorInfo.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var data = await _context.TblFloorInfo
            .FirstOrDefaultAsync(x => x.Flid == id);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Floor Info not found.";
            return RedirectToAction(nameof(FloorInfoList));
        }

        _context.TblFloorInfo.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Floor Info deleted successfully.";

        return RedirectToAction(nameof(FloorInfoList));
    }

}


