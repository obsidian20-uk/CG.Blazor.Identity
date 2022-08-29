
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="ResetPasswordPage"/>
/// page.
/// </summary>
public partial class ResendEmailConfimationPage
{
    public string StatusMessage { get; set; }

    public EmailChangeModel _model { get; set; }

    [Inject]
    public NavigationManager NavMan { get; set; }

    [Inject]
    public IEmailSender EmailSender { get; set; }

    [Inject]
    protected UserManager<IdentityUser> UserMan { get; set; } = null!;

    protected async Task OnSubmit(
        EditContext context
        )
    {
        var user = await UserMan.FindByEmailAsync(_model.Email);
        if (user == null)
        {
            StatusMessage = "Verification email sent. Please check your email.";
        }

        var code = await UserMan.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var callbackUrl = $"{NavMan.BaseUri}Account/ConfirmEmail?userId={user.Id}&code={code}";

        await EmailSender.SendEmailAsync(_model.Email, "SocialMeeple - Confirm your email",
$"Hi, {user.Email}, <p> You have recently registered for an account, if this was not you or was in error then please ignore this email else please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>. </p> <p>Thanks Social Meeple</p>");
    }
}
