namespace CG.Blazor.Identity.ViewModels;

/// <summary>
/// This class is a temporary view model, used to pass state between the 
/// UI and middlewear.
/// </summary>
/// <typeparam name="TUser">The type of associated user.</typeparam>
internal class CacheVM<TUser> where TUser : IdentityUser
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains the associated user.
    /// </summary>
    public TUser User { get; set; } = null!;

    /// <summary>
    /// This property contains the password for a user.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// This property indicates whether the write a cookie, as part
    /// of the overall login operation.
    /// </summary>
    public bool RememberMe { get; set; }

    /// <summary>
    /// This property indicates whether to lockout the user on 
    /// a failed login operation.
    /// </summary>
    public bool LockoutOnFailure { get; set; }

    #endregion

}
