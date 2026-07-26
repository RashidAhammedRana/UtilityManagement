using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UtilityManagement.Data;
using UtilityManagement.Models;


public class EquipmentDetailsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public EquipmentDetailsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EquipmentDetailsList(int page = 1, string searchString = "")
    {
        int pageSize = 15;

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var menuId = await _context.TblMenu
            .Where(x => x.MenuName == "Equipment Information")
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

        var currentLocation = _context.Users
            .Where(x => x.Id == userId)
            .Select(x => x.Company)
            .FirstOrDefault();
        //var query = _context.TblEquipmentDetails
        //    .Where(x => x.CurrentLocation == currentLocation);

        var query = _context.TblEquipmentDetails
            .Where(x => string.IsNullOrEmpty(currentLocation) || x.CurrentLocation == currentLocation);

        ViewBag.EquipmentList = query
            .Select(x => new SelectListItem
            {
                Value = x.Eqid.ToString(),
                Text = $"{x.EquipmentName} - {x.CurrentLocation}"
            })
            .ToList();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            searchString = searchString.Trim();

            query = query.Where(x =>
                x.EquipmentName.Contains(searchString) ||
                x.Brand.Contains(searchString) ||
                x.Model.Contains(searchString) ||
                x.CurrentLocation.Contains(searchString)
            );
        }

        var totalRecords = await query.CountAsync();

        var data = await query
            .OrderBy(x => x.EquipmentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.SearchString = searchString;
        ViewBag.totalEquipments = totalRecords;

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> CreateAsync()
    {
        ViewBag.CompanyList = _context.TblCompanyInfo
            .Select(x => new SelectListItem
            {
                Value = x.ComName,
                Text = $"{x.ComName}"
            })
            .ToList();
        // Check Admin User
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        ViewBag.IsAdmin = user != null && user.UserName == "admin.bpa";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblEquipmentDetail equipmentDetails)
    {
        try
        {
            if (equipmentDetails.Status == "Inactive" &&
           string.IsNullOrWhiteSpace(equipmentDetails.Remarks))
            {
                ModelState.AddModelError("Remarks",
                    "Remarks is required when Status is Inactive.");
            }
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill all required fields.";
                return View(equipmentDetails);
            }

            // ✅ CHECK DUPLICATE SL NO
            var isExist = await _context.TblEquipmentDetails
                .AnyAsync(x => x.Slno == equipmentDetails.Slno);

            if (isExist)
            {
                ModelState.AddModelError("SlNo", "This SL No already exists!");
                return View(equipmentDetails);
            }
            var now = DateTime.Now;
            var currentUser = User.Identity?.Name ?? "System";
            // Created Information
            equipmentDetails.CreatedAt = now;
            equipmentDetails.CreatedBy = currentUser;
            _context.TblEquipmentDetails.Add(equipmentDetails);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Equipment created successfully.";

            return RedirectToAction(nameof(EquipmentDetailsList));
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Failed to create equipment.";
            return View(equipmentDetails);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var equipments = await _context.TblEquipmentDetails
            .FirstOrDefaultAsync(x => x.Eqid == id);

        if (equipments == null)
        {
            return NotFound();
        }


        // Company dropdown
        ViewBag.CompanyList = await _context.TblCompanyInfo
            .Select(x => new SelectListItem
            {
                Value = x.ComName,
                Text = x.ComName
            })
            .ToListAsync();


        // Check Admin User
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == userId);

        ViewBag.IsAdmin = user != null && user.UserName == "admin.bpa";


        return View(equipments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblEquipmentDetail equipmentDetails)
    {
        // Status Inactive হলে Remarks required
        if (equipmentDetails.Status == "Inactive" &&
            string.IsNullOrWhiteSpace(equipmentDetails.Remarks))
        {
            ModelState.AddModelError("Remarks",
                "Remarks is required when Status is Inactive.");
        }
        if (!ModelState.IsValid)
            return View(equipmentDetails);

        var data = await _context.TblEquipmentDetails
            .FirstOrDefaultAsync(x => x.Eqid == equipmentDetails.Eqid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Equipment not found.";
            return RedirectToAction(nameof(EquipmentDetailsList));
        }

        data.EquipmentName = equipmentDetails.EquipmentName;
        data.Capacity = equipmentDetails.Capacity;
        data.Brand = equipmentDetails.Brand;
        data.Model = equipmentDetails.Model;
        data.Slno = equipmentDetails.Slno;
        data.CurrentLocation = equipmentDetails.CurrentLocation;
        data.Status = equipmentDetails.Status;
        data.Remarks = equipmentDetails.Remarks;
        // Update audit fields
        data.UpdatedAt = DateTime.Now;
        data.UpdatedBy = User.Identity?.Name ?? "System";
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Equipment updated successfully.";

        return RedirectToAction(nameof(EquipmentDetailsList));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var data = await _context.TblEquipmentDetails.FindAsync(id);

        if (data == null)
            return NotFound();

        return View(data);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int eqid)
    {
        var data = await _context.TblEquipmentDetails
            .FirstOrDefaultAsync(x => x.Eqid == eqid);

        if (data == null)
        {
            TempData["ErrorMessage"] = "Equipment not found.";
            return RedirectToAction(nameof(EquipmentDetailsList));
        }

        _context.TblEquipmentDetails.Remove(data);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Equipment deleted successfully.";

        return RedirectToAction(nameof(EquipmentDetailsList));
    }
}

