
namespace CG.Blazor.Identity.Models;

/// <summary>
/// This class represents the input for a login operation.
/// </summary>
public class LoginPageModel
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains an emial, or user name for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Email or user name")]
    public string EmailOrUserName { get; set; } = null!;

    /// <summary>
    /// This property contains a password for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// This property indicates the login should be persistent.
    /// </summary>
    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    #endregion
}
