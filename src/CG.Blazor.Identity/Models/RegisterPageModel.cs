
namespace CG.Blazor.Identity.Models;

/// <summary>
/// This class represents the input for a registration operation.
/// </summary>
public class RegisterPageModel
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains a user name for a registration.
    /// </summary>
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// This property contains an email address for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// This property contains a password for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// This property contains a confirmation password for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = null!;

    #endregion
}
