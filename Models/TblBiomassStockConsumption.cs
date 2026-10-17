using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilityManagement.Models;

public partial class TblBiomassStockConsumption
{
    public int Trid { get; set; }

    public DateOnly? Trdate { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Company { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? SewingJhutTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? DenimJhutTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CharcoalTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? CartonTotalCost { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? PpTotalCost { get; set; }
    public double? KnitJhutOpening { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutReceive { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutB1Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutB2Cons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutTotalCons { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutClosing { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public double? KnitJhutTotalCost { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
