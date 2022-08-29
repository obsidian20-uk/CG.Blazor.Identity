
using Microsoft.AspNetCore.Components.Forms;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Shared;

/// <summary>
/// This class is the code-behind for the <see cref="LoginLayout"/> page.
/// </summary>
public partial class LoginLayout
{
    /// <summary>
    /// This property contains blazor identity options for the page.
    /// </summary>
    [Inject]
    protected IOptions<BlazorIdentityOptions> Options { get; set; }

    public string pageStyle { get; set; }

    protected override void OnInitialized()
    {
        if (Options.Value.Theme.BackgroundImage == "")
        {
            pageStyle = $"background-color: {Options.Value.Theme.GutterColor};";
        }
        else
        {
            pageStyle = $"background-image: {Options.Value.Theme.BackgroundImage};";
        }
    }
}
