
namespace CG.Blazor.Identity.Models;

/// <summary>
/// This class represents the results of a register operation.
/// </summary>
public class RegisterResult
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains the collection of associated error messages.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// This property indicates whether the operation was successful.
    /// </summary>
    public bool Succeeded { get; set; } = true;

    #endregion
}
