
namespace CG.Blazor.Identity.Areas.Identity;

/// <summary>
/// This class is a service that receives authentication state from the
/// host environment, and revalidates it at regular intervals.
/// </summary>
/// <typeparam name="TUser">The type of associtaed user.</typeparam>
public class RevalidatingIdentityAuthenticationStateProvider<TUser>
    : RevalidatingServerAuthenticationStateProvider where TUser : class
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains a scope factory for the provider.
    /// </summary>
    private readonly IServiceScopeFactory _scopeFactory;

    /// <summary>
    /// This field contains identity options for the provider.
    /// </summary>
    private readonly IdentityOptions _options;

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains the revalidation interval.
    /// </summary>
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This class creates a new instance of the <see cref="RevalidatingIdentityAuthenticationStateProvider{TUser}"/>
    /// class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory to use with this 
    /// provider.</param>
    /// <param name="scopeFactory">The scope factory to use with this provider.</param>
    /// <param name="optionsAccessor">The options accessor to use with this
    /// provider.</param>
    public RevalidatingIdentityAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        // Save the reference(s).
        _scopeFactory = scopeFactory;
        _options = optionsAccessor.Value;
    }

    #endregion

    // *******************************************************************
    // Protected methods.
    // *******************************************************************

    #region Protected methods

    /// <summary>
    /// This method determines whether the authentication state is still valid.
    /// </summary>
    /// <param name="authenticationState">The current <see cref="AuthenticationState"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> 
    /// to observe while performing the operation.</param>
    /// <returns>A task that resolves as true if the <paramref name="authenticationState"/> 
    /// is still valid, or false if it is not.</returns>
    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, 
        CancellationToken cancellationToken
        )
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        var scope = _scopeFactory.CreateScope();
        try
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            
            return await ValidateSecurityStampAsync(
                userManager, 
                authenticationState.User
                );
        }
        finally
        {
            if (scope is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                scope.Dispose();
            }
        }
    }

    #endregion

    // *******************************************************************
    // Private methods.
    // *******************************************************************

    #region Private methods

    /// <summary>
    /// This validates the current user's security stamp.
    /// </summary>
    /// <param name="userManager">The user manager to use for the operation.</param>
    /// <param name="principal">The claims principal to use for the operation.</param>
    /// <returns>A task that resolves as true if the user's security stamp
    /// is still valid, or false otherwise.</returns>
    private async Task<bool> ValidateSecurityStampAsync(
        UserManager<TUser> userManager, 
        ClaimsPrincipal principal
        )
    {
        var user = await userManager.GetUserAsync(principal);
        if (user == null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            var principalStamp = principal.FindFirstValue(
                _options.ClaimsIdentity.SecurityStampClaimType
                );

            var userStamp = await userManager.GetSecurityStampAsync(
                user
                );

            return principalStamp == userStamp;
        }
    }

    #endregion
}
