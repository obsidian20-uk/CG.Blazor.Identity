
namespace CG.Blazor.Identity.Models;

/// <summary>
/// This class represents the input for a login operation.
/// </summary>
/// 

public class EmailChangeModel
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains an emial, or user name for a registration.
    /// </summary>
    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    #endregion
}
