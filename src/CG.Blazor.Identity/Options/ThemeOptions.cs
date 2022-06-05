
namespace CG.Blazor.Identity.Options;

/// <summary>
/// This class contains configuration settings related to visual themes.
/// </summary>
public class ThemeOptions
{
    /// <summary>
    /// This property contains the default gutter color.
    /// </summary>
    [Required]
    public string GutterColor { get; set; } = Constants.GutterColor;

    /// <summary>
    /// This property contains the default background color.
    /// </summary>
    [Required]
    public string BackgroundColor { get; set; } = Constants.BackgroundColor;
}
