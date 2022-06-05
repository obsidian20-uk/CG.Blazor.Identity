
namespace CG.Blazor.Identity.Shared
{
    /// <summary>
    /// This component redirects if the current user is not authenticated.
    /// </summary>
    public class RedirectToLogin : ComponentBase
    {
        // *******************************************************************
        // Properties.
        // *******************************************************************

        #region Properties
        
        /// <summary>
        /// This property contains the navigation manager to use with this component.
        /// </summary>
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = null!;

        /// <summary>
        /// This property contains the blazor identity options to use with this component.
        /// </summary>
        [Inject]
        protected IOptions<BlazorIdentityOptions> Options { get; set; } = null!;

        /// <summary>
        /// This property contains the optional return url.
        /// </summary>
        [Parameter]
        public string? ReturnUrl { get; set; }

        #endregion

        // *******************************************************************
        // Protected methods.
        // *******************************************************************

        #region Protected methods

        /// <summary>
        /// This method is called after Blazor preforms a render.
        /// </summary>
        /// <param name="firstRender">True is this is the first render; false
        /// otherwise.</param>
        protected override void OnAfterRender(bool firstRender)
        {
            // Give the base class a chance.
            base.OnAfterRender(firstRender);

            // Get the return url.
            var returnUrl = Uri.EscapeDataString(ReturnUrl ?? NavigationManager.Uri);

            // We've redirected here, instead of using OnInitialize, in order
            //   to work around a Blazor issue where NavigateTo throws an exception
            //   for no apparent reason.
            NavigationManager.NavigateTo(
                $"{Options.Value.Endpoints.LoginEndPoint}?returnUrl={returnUrl}"
                );
        }

        #endregion
    }
}
