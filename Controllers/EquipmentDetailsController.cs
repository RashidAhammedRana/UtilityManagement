using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UtilityManagement.Data;
using UtilityManagement.Models;


public class EquipmentDetailsController : Controller
{
    private readonly ApplicationDbContext _context;

    public EquipmentDetailsController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> EquipmentDetailsList(int page = 1, int pageSize = 15)
    {
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

        // 📄 Pagination data
        var equipmentsDetails = await _context.TblEquipmentDetails
            .OrderBy(r => r.EquipmentName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalRecords = await _context.TblEquipmentDetails.CountAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        ViewBag.TotalEquipmentDetails = totalRecords;

        return View(equipmentsDetails);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblEquipmentDetails equipmentDetails)
    {
        try
        {
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

        var equipments = await _context.TblEquipmentDetails.FindAsync(id);
        if (equipments == null)
        {
            return NotFound();
        }
        return View(equipments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblEquipmentDetails equipmentDetails)
    {
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

