
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="RegisterPage"/>
/// page.
/// </summary>
public partial class RegisterPage
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains the model for the form.
    /// </summary>
    protected readonly RegisterPageModel _model = new();

    /// <summary>
    /// This field contains a status message.
    /// </summary>
    protected List<string> _status = new();

    /// <summary>
    /// This field contains an error message.
    /// </summary>
    protected List<string> _errors = new();

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains a logger for the page.
    /// </summary>
    [Inject]
    protected ILogger<LoginPage> Logger { get; set; } = null!;

    /// <summary>
    /// This property contains a blazor identity manager for the page.
    /// </summary>
    [Inject]
    protected BlazorIdentityManager<IdentityUser> BlazorIdentityManager { get; set; } = null!;

    /// <summary>
    /// This property contains blazor identity options for the page.
    /// </summary>
    [Inject]
    protected IOptions<BlazorIdentityOptions> Options { get; set; } = null!;

    /// <summary>
    /// This property contains a list of optional external login schemes.
    /// </summary>
    protected IList<AuthenticationScheme>? ExternalLogins { get; set; }

    #endregion

    // *******************************************************************
    // Protected methods.
    // *******************************************************************

    #region Protected methods

    /// <summary>
    /// This method is called by Blazor when the page is initialized.
    /// </summary>
    /// <returns>A task to perform the operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        // Fill the list of external login schemes.
        ExternalLogins = (await BlazorIdentityManager.SignInManager.GetExternalAuthenticationSchemesAsync()
            .ConfigureAwait(false)
            ).ToList();

        // Give the base class a chance.
        await base.OnInitializedAsync()
            .ConfigureAwait(false);
    }

    // *******************************************************************

    /// <summary>
    /// This method is called when the user submits the edit form.
    /// </summary>
    /// <param name="context">The edit context to use for the operation.</param>
    /// <returns>A task to perform the operation.</returns>
    protected async Task OnSubmit(
        EditContext context
        )
    {
        try
        {
            // Get rid of any old state.
            _status.Clear();
            _errors.Clear();

            // Update the UI.
            await InvokeAsync(
                () => StateHasChanged()
                ).ConfigureAwait(false);

            // Sanity check the model.
            if (!context.Validate())
            {
                return; // Nothing to do!
            }

            // Defer to the manager for the registration.
            var result = await BlazorIdentityManager.RegisterAsync(
                _model.UserName,
                _model.Email,
                _model.Password,
                _model.ConfirmPassword
                ).ConfigureAwait(false);

            // Did we fail?
            if (!result.Succeeded)
            {
                // Record the errors.
                _errors.AddRange(result.Errors);
            }
        }
        catch (Exception ex)
        {
            // Remember the error.
            _status.Clear();
            _errors.Add("Internal failure! Please contact customer support");

            // Tell the world what happened.
            Logger.LogError(
                ex,
                "Failed to register user: {user}!", _model.UserName
                );
        }
    }

    #endregion
}
