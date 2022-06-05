
namespace CG.Blazor.Identity.Middlewear;

/// <summary>
/// This class supports our Blazor/Razor based identity operations.
/// </summary>
/// <typeparam name="TUser">The type of associated user.</typeparam>
 internal class BlazorIdentityModule<TUser> 
    where TUser : IdentityUser
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains the next module in the pipeline.
    /// </summary>
    private readonly RequestDelegate _next;

    /// <summary>
    /// This field contains the options for this manager.
    /// </summary>
    protected readonly IOptions<BlazorIdentityOptions> _options;

    /// <summary>
    /// This field contains a logger for the module.
    /// </summary>
    private readonly ILogger<BlazorIdentityModule<TUser>> _logger;

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="BlazorIdentityModule{TUser}"/>
    /// class.
    /// </summary>
    /// <param name="next">The next module in the pipeline.</param>
    /// <param name="options">The options to use with this module.</param>
    /// <param name="logger">The logger to use with this module.</param>
    public BlazorIdentityModule(
        RequestDelegate next,
        IOptions<BlazorIdentityOptions> options,
        ILogger<BlazorIdentityModule<TUser>> logger
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(next, nameof(next))
            .ThrowIfNull(options, nameof(options))
            .ThrowIfNull(logger, nameof(logger));

        // Save the reference(s).
        _next = next;
        _options = options;
        _logger = logger;
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    public async Task Invoke(
        HttpContext context, 
        SignInManager<TUser> signInManager
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(context, nameof(context))
            .ThrowIfNull(signInManager, nameof(signInManager));

        // Have we called the login endpoint usinG our key?
        if (context.Request.Path.StartsWithSegments(
            _options.Value.Endpoints.LoginEndPoint
            ) && context.Request.Query.ContainsKey("key"))
        {
            // Get the key from the request.
            var key = context.Request.Query["key"];
                
            // Is the key missing?
            if (string.IsNullOrEmpty(key))
            {
                // Tell the world what happened.
                _logger.LogWarning("User key: was missing from the request!");
            }

            // Is the user in our cache?
            if (BlazorIdentityManager<TUser>._userCache.TryGetValue(
                key, 
                out var userVM
                ))
            {
                // Attempt to log the user in.
                var result = await signInManager.PasswordSignInAsync(
                    userVM.User,
                    userVM.Password,
                    userVM.RememberMe,
                    userVM.LockoutOnFailure
                    );

                // Once we tried to log in, we no longer need the password.
                userVM.Password = ""; 

                // Did the login succeed?
                if (result.Succeeded)
                {
                    // Once we're logged in, we no longer need the cached user.
                    BlazorIdentityManager<TUser>._userCache.Remove(key, out _);

                    // Was a return url specified?
                    if (context.Request.Query.ContainsKey("returnUrl"))
                    {
                        // Get the return url.
                        var returnUrl = context.Request.Query["returnUrl"];

                        // Perform the redirect.
                        context.Response.Redirect(returnUrl);
                    }
                    else
                    {
                        // Otherwise, redirect to the home page.
                        context.Response.Redirect("/");
                    }
                    return; // We redirected so don't call _next.Invoke
                }
            }
            else
            {
                // Tell the world what happened.
                _logger.LogWarning("User key: {key} was invalid!", key);
            }
        }

        // Have we called the logout endpoint?
        else if (context.Request.Path.StartsWithSegments(
            _options.Value.Endpoints.LogoutEndPoint
            ))
        {
            // Sign the current user out.
            await signInManager.SignOutAsync();

            // Redirect to the home page.
            context.Response.Redirect("/");

            return; // We redirected so don't call _next.Invoke
        }

        // By default, call the next module in the pipeline.
        await _next.Invoke(context);
    }

    #endregion
}

