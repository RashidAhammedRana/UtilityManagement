using System.ComponentModel.DataAnnotations;
namespace UtilityManagement.ViewModels;

public class RegisterViewModel
{
    [Required]
    //[EmailAddress]
    [Display(Name = "Username")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
