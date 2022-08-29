
using Microsoft.AspNetCore.Components.Forms;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="LoginPage"/> page.
/// </summary>
public partial class LoginPage
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains a reference to the edit form.
    /// </summary>
    protected EditForm? _editForm;

    /// <summary>
    /// This field contains the model for the form.
    /// </summary>
    protected readonly LoginPageModel _model = new();

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

    #endregion

    // *******************************************************************
    // Protected methods.
    // *******************************************************************

    #region Protected methods

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

            // Defer to the manager for the login.
            var result = await BlazorIdentityManager.PasswordSignInAsync(
                _model.EmailOrUserName,
                _model.Password,
                _model.RememberMe,
                true
                ).ConfigureAwait(false);

            // NOTE: if result.Succeeded is true then we'll be redirected.

            // Did we fail?
            if (!result.Succeeded)
            {
                // Decide what to show the user.
                if (result.IsLockedOut)
                {
                    _status.Add("Your account is currently locked out.");
                }
                else if (result.IsNotAllowed)
                {
                    _status.Add("Please ensure your email is verified.");
                }
                else
                {
                    _status.Add("Please check your credentials.");
                }
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
                "Failed to log user: {user} in!", _model.EmailOrUserName
                );
        }
    }

    #endregion
}
