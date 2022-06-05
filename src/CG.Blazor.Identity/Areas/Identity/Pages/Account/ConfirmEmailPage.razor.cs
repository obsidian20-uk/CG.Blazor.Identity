using System.Web;

namespace CG.Blazor.Identity.Areas.Identity.Pages.Account;

/// <summary>
/// This class is the code-behind for the <see cref="ConfirmEmailPage"/> page.
/// </summary>
public partial class ConfirmEmailPage
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

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
    /// This property contains a navigation manager for the page.
    /// </summary>
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    #endregion

    // *******************************************************************
    // Protected methods.
    // *******************************************************************

    #region Protected methods

    /// <summary>
    /// This method is called by Blazor after the page is rendered.
    /// </summary>
    /// <returns>A task to perform the operation.</returns>
    protected override async Task OnAfterRenderAsync(
        bool firstRender
        )
    {
        // NOTE : we use the OnAfterRenderAync method, instead of the
        //   OnInitializeAsync method, because Blazor has a known bug
        //   whereby, calling NavigationManager.NavigateTo from inside
        //   the OnInitializeAsync method throws an exception.

        // Is this the first render?
        if (firstRender)
        {
            try
            {
                // Get the current url.
                var uri = new Uri(NavigationManager.Uri);

                // Are the parameters missing?
                if (string.IsNullOrEmpty(uri.Query))
                {
                    // Navigate to the root.
                    NavigationManager.NavigateTo("/");
                    return;
                }
                else
                {
                    // Parse the parameters.
                    var parms = HttpUtility.ParseQueryString(uri.Query);
                    var userId = parms["userId"];
                    var code = parms["code"];

                    // Sanity check the parameters.
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userId))
                    {
                        // Navigate to the root.
                        NavigationManager.NavigateTo("/");
                        return;
                    }
                    else
                    {
                        // Defer to the manager for the search.
                        var user = await BlazorIdentityManager.SignInManager.UserManager.FindByIdAsync(
                            userId
                            ).ConfigureAwait(false);

                        // Did we fail?
                        if (null == user)
                        {
                            // Tell the caller what happened.
                            _errors.Add("Unable to confirm your email.");
                        }
                        else
                        {
                            // Decode the code.
                            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(
                                code
                                ));

                            // Defer to the manager for the confirmation.
                            var result = await BlazorIdentityManager.SignInManager.UserManager.ConfirmEmailAsync(
                                user,
                                code
                                ).ConfigureAwait(false);

                            // Did we succeed?
                            if (result.Succeeded)
                            {
                                // Tell the caller what happened.
                                _status.Add("Thank you for confirming your email.");
                            }
                            else
                            {
                                // Tell the caller what happened.
                                _errors.Add("Error confirming your email.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Tell the world what happened.
                Logger.LogError(
                    ex,
                    "Failed to confirm an email"
                    );

                // Tell the caller what happened.
                _errors.Add("Error confirming your email.");
            }

            // Update the UI.
            await InvokeAsync(() => StateHasChanged())
                .ConfigureAwait(false);
        }

        // Give the base class a chance.
        await base.OnAfterRenderAsync(
            firstRender
            ).ConfigureAwait(false);
    }

    #endregion
}
