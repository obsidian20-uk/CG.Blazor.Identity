
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="ResetPasswordPage"/>
/// page.
/// </summary>
public partial class ResetPasswordPage
{
    public string StatusMessage { get; set; }

    public ResetPasswordModel _model { get; set; } = new ResetPasswordModel();

    /// <summary>
    /// This property contains a navigation manager for the page.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    protected UserManager<IdentityUser> UserMan { get; set; } = null!;

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("code", out var _code))
        {
            _model.code = _code;
        }

        if (_model.code == null)
        {
            StatusMessage = "A code must be supplied for password reset.";
        }
        else
        {
            _model = new ResetPasswordModel
            {
                code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(_model.code))
            };
        }
    }

    protected async Task OnSubmit(
        EditContext context
        )
    {
        var user = await UserMan.FindByNameAsync(_model.UserName);
        if (user == null)
        {
            StatusMessage = "Your password has been reset. Please <a href='/Account/Login'> click here to log in</a>.";
        }

        var result = await UserMan.ResetPasswordAsync(user, _model.code, _model.Password);
        if (result.Succeeded)
        {
            StatusMessage = "Your password has been reset. Please <a href='/Account/Login'> click here to log in</a>.";
        }
        else if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
        {
            StatusMessage = "Passwords do no match";
        }
    }
}
