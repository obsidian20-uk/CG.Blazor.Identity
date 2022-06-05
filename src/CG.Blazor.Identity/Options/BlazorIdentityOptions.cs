
namespace CG.Blazor.Identity.Options;

/// <summary>
/// This class contains configuration settings related to Blazor identity 
/// operations.
/// </summary>
public class BlazorIdentityOptions
{
    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains endpoint options.
    /// </summary>
    public EndpointOptions Endpoints { get; set; } = new();

    /// <summary>
    /// This property contains the theme options.
    /// </summary>
    public ThemeOptions Theme { get; set; } = new();

    #endregion
}
