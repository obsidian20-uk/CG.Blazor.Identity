using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Web;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="ForgotPasswordPage"/> page.
/// </summary>
public partial class ForgotPasswordPage
{
    public string StatusMessage { get; set; }

    /// <summary>
    /// This field contains the model for the form.
    /// </summary>
    protected readonly EmailChangeModel _model = new();

    public bool NoUser { get; set; }

    [Inject]
    protected BlazorIdentityManager<IdentityUser> BlazorIdentityManager { get; set; } = null!;

    protected async Task OnSubmit(
        EditContext context
        )
    {
        var user = await UserMan.FindByEmailAsync(_model.Email);
        if (user == null || !(await UserMan.IsEmailConfirmedAsync(user)))
        {
            NoUser = true;
            return;
        }

        await BlazorIdentityManager.SendForgotPasswordEmail(user);
    }
}
