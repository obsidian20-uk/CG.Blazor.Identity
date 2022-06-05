
namespace CG.Blazor.Identity.Services;

/// <summary>
/// This class is an object that manages Blazor/Razor based identity operations.
/// </summary>
/// <typeparam name="TUser">The type of associated user.</typeparam>
public class BlazorIdentityManager<TUser>
    where TUser : IdentityUser
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains the options for this manager.
    /// </summary>
    protected readonly IOptions<BlazorIdentityOptions> _options;

    /// <summary>
    /// This field contains the logger for this manager.
    /// </summary>
    protected readonly ILogger<BlazorIdentityManager<TUser>> _logger;

    /// <summary>
    /// This field contains the inner signin manager for this manager.
    /// </summary>
    protected readonly SignInManager<TUser> _signInManager;

    /// <summary>
    /// This field contains the navigation manager for this manager.
    /// </summary>
    protected readonly NavigationManager _navigationManager;

    /// <summary>
    /// This field contains an email sender for this manager.
    /// </summary>
    protected readonly IEmailSender _emailSender;

    /// <summary>
    /// This field contains a shared cache for short-term storage while 
    /// a login operation is in progress.
    /// </summary>
    internal static readonly ConcurrentDictionary<string, CacheVM<TUser>> _userCache = new();

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property allows access to the inner sign-in manager.
    /// </summary>
    public SignInManager<TUser> SignInManager => _signInManager;

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="BlazorIdentityManager{TUser}"/>
    /// class.
    /// </summary>
    /// <param name="options">The options to use with this manager.</param>
    /// <param name="logger">The logger to use with this manager.</param>
    /// <param name="signInManager">The ASP.NET sign in manager to use with 
    /// this manager.</param>
    /// <param name="navigationManager">The navigation manager to use with 
    /// this manager.</param>
    /// <param name="emailSender">The email sender to use for the operation.</param>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public BlazorIdentityManager(
        IOptions<BlazorIdentityOptions> options,
        ILogger<BlazorIdentityManager<TUser>> logger,
        SignInManager<TUser> signInManager,
        NavigationManager navigationManager,
        IEmailSender emailSender
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(options, nameof(options))
            .ThrowIfNull(logger, nameof(logger))
            .ThrowIfNull(signInManager, nameof(signInManager))
            .ThrowIfNull(navigationManager, nameof(navigationManager))
            .ThrowIfNull(emailSender, nameof(emailSender));

        // Save the reference(s).
        _options = options;
        _logger = logger;
        _signInManager = signInManager;
        _navigationManager = navigationManager;
        _emailSender = emailSender; 
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods
    
    /// <summary>
    /// This method attempts to sign in the given user, using the specified
    /// user name / email, and password. If the operation is successful, a
    /// redirect takes place to the associated middlewear module, to perform
    /// the actual login and (optionally) write the cookie.
    /// </summary>
    /// <param name="emailOrUserName">The email address, or user name, to
    /// be used for the operation.</param>
    /// <param name="password">The password to by used for the operation.</param>
    /// <param name="isPersistent">True to direct the middlewear module to 
    /// write out the identity cookie; false otherwise.</param>
    /// <param name="lockoutOnFailure">True to direct the ASP.NET sign in
    /// manager to lockout the account on failure; false otherwise.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task to perform the operation, that returns a <see cref="SignInResult"/> 
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// an argument is missing, or invalid.</exception>
    public virtual async ValueTask<SignInResult> PasswordSignInAsync(
        string emailOrUserName, 
        string password,
        bool isPersistent,
        bool lockoutOnFailure,
        CancellationToken cancellationToken = default
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNullOrEmpty(emailOrUserName, nameof(emailOrUserName))
            .ThrowIfNullOrEmpty(password, nameof(password));

        // Tell the world what we are doing.
        _logger.LogDebug("Looking for a user by email: {email}", emailOrUserName);

        // Ask ASP.NET for the user, searching by email.
        var user = await _signInManager.UserManager.FindByEmailAsync(
            emailOrUserName
            ).ConfigureAwait(false);

        // Did we fail?
        if (null == user)
        {
            // Tell the world what we are doing.
            _logger.LogDebug("Looking for a user by name: {name}", emailOrUserName);

            // Ask ASP.NET for the user, searching by user name.
            user = await _signInManager.UserManager.FindByNameAsync(
                emailOrUserName
                ).ConfigureAwait(false);

            // Did we fail?
            if (null == user)
            {
                // Tell the world what happened.
                _logger.LogWarning(
                    "User: {un} not found!", emailOrUserName
                    );

                // Return failure.
                return SignInResult.NotAllowed;
            }
        }

        // Tell the world what we are doing.
        _logger.LogDebug("Checking if we can sign this user in");
        
        // Ask ASP.NET if this user can login.
        var canSignIn = await _signInManager.CanSignInAsync(
            user
            ).ConfigureAwait(false);

        // Did we fail?
        if (!canSignIn)
        {
            // Tell the world what happened.
            _logger.LogWarning(
                "User: {un} is not allowed to login!", 
                emailOrUserName
                );

            // Return failure.
            return SignInResult.NotAllowed;
        }

        // Tell the world what we are doing.
        _logger.LogDebug("Checking password for user: {name}", user.Email);

        // Ask ASP.NET if this password is correct.
        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, 
            password,
            lockoutOnFailure
            );

        // Did we succeed?
        if (signInResult.Succeeded)
        {
            // Tell the world what we are doing.
            _logger.LogDebug("Looking for the return url, if any");

            // Try to recover the return url, if there is one.
            var returnUrl = "";
            var uri = new Uri(_navigationManager.Uri);
            if (!string.IsNullOrEmpty(uri.Query))
            {
                if (uri.Query.StartsWith("?returnUrl="))
                {
                    // Get the value of the parameter.
                    returnUrl = uri.Query["?returnUrl=".Length..];
                }
            }

            // Tell the world what we are doing.
            _logger.LogDebug("Creating new VM for user: {email}", user.Email);

            // Create a model for the user.
            var userVM = new CacheVM<TUser>()
            {
                User = user,
                Password = password,
                RememberMe = isPersistent,
                LockoutOnFailure = lockoutOnFailure,
            };

            // Tell the world what we are doing.
            _logger.LogDebug("Creating new key for user: {email}", user.Email);

            // Create a unique key for the user.
            var userKey = $"{Guid.NewGuid()}";

            // Tell the world what we are doing.
            _logger.LogDebug("Caching user: {email}, {key}", user.Email, userKey);

            // Add the model our temporary cache.
            _userCache.AddOrUpdate(
                userKey,
                userVM,
                (key, current) => userVM
                );

            // Tell the world what we are doing.
            _logger.LogDebug("Calculating redirect");

            // How should we redirect?
            if (string.IsNullOrEmpty(returnUrl))
            {
                // Redirect with a key and no return url.
                _navigationManager.NavigateTo(
                    $"{_options.Value.Endpoints.LoginEndPoint}?key={userKey}", 
                    true
                    );
            }
            else 
            {
                // Redirect with a key and a return url.
                _navigationManager.NavigateTo(
                    $"{_options.Value.Endpoints.LoginEndPoint}?key={userKey}&returnUrl={returnUrl}", 
                    true
                    );
            }            
        }

        // Return the results.
        return signInResult;
    }

    // *******************************************************************

    /// <summary>
    /// This method attempts to register a new user, using the specified 
    /// user name / email, and password. If the operation is successful, a
    /// redirect takes place to either the (A) confirmation page, or (B) 
    /// the associated middlewear module.
    /// </summary>
    /// <param name="userName">The user name to use for the operation.</param>
    /// <param name="email">The email to use for the operation.</param>
    /// <param name="password">The password to use for the operation.</param>
    /// <param name="confirmPassword">The confirmation password to use for
    /// the operation.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task to perform the operation, that returns a <see cref="RegisterResult"/> 
    /// object.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public virtual async ValueTask<RegisterResult> RegisterAsync(
        string userName,
        string email,
        string password,
        string confirmPassword,
        CancellationToken cancellationToken = default
        ) 
    {
        var result = new RegisterResult();

        // Tell the world what we are doing.
        _logger.LogDebug("Confirming the passwords are identical");

        // Sanity check the passwords.
        if (!password.Equals(confirmPassword))
        {
            // Tell the caller what happened.
            result.Succeeded = false;
            result.Errors.Add("The passwords don't match!");

            // Return the results.
            return result;
        }

        // Tell the world what we are doing.
        _logger.LogDebug("Looking for a user by name: {name}", userName);

        // Ask ASP.NET for the user, searching by user name.
        var user = await _signInManager.UserManager.FindByNameAsync(
            userName
            ).ConfigureAwait(false);

        // Did we fail?
        if (null != user)
        {
            // Tell the world what happened.
            _logger.LogWarning(
                "User: {un} already exists!", userName
                );

            // Tell the caller what happened.
            result.Succeeded = false;
            result.Errors.Add("The user name is already taken!");

            // Return the results.
            return result;
        }

        // Tell the world what we are doing.
        _logger.LogDebug("Looking for a user by email: {email}", email);

        // Ask ASP.NET for the user, searching by email.
        user = await _signInManager.UserManager.FindByEmailAsync(
            email
            ).ConfigureAwait(false);

        // Did we fail?
        if (null != user)
        {
            // Tell the world what happened.
            _logger.LogWarning(
                "Email: {email} already exists!", email
                );

            // Tell the caller what happened.
            result.Succeeded = false;
            result.Errors.Add("The email is already taken!");

            // Return the results.
            return result;
        }

        // Tell the world what we are doing.
        _logger.LogDebug("Creating a new user");

        // Create a new user model.
        var newUser = Activator.CreateInstance<TUser>();

        // Fill in the required properties.
        newUser.UserName = userName;
        newUser.Email = email;

        // Defer to the user manager to create the user.
        var createResult = await _signInManager.UserManager.CreateAsync(
            newUser,
            password
            ).ConfigureAwait(false);

        // Did we succeed?
        if (createResult.Succeeded)
        {
            // Tell the world what we are doing.
            _logger.LogDebug("Checking for a return url");

            // Try to recover the return url, if there is one.
            var returnUrl = "";
            var uri = new Uri(_navigationManager.Uri);
            if (!string.IsNullOrEmpty(uri.Query))
            {
                // Does the query contain our parameter?
                if (uri.Query.Contains("returnUrl="))
                {
                    // Get the value of the parameter.
                    returnUrl = uri.Query["?returnUrl=".Length..];
                }
            }

            // Tell the world what we are doing.
            _logger.LogDebug("Checking for the user's identifier");

            // Get the id for the new user.
            var userId = await _signInManager.UserManager.GetUserIdAsync(
                newUser
                ).ConfigureAwait(false);

            // Tell the world what we are doing.
            _logger.LogDebug("Checking for the user's confirmation token");

            // Get a confirmation token for the new user.
            var code = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(
                newUser
                ).ConfigureAwait(false);

            // Tell the world what we are doing.
            _logger.LogDebug("Encoding the user's confirmation token");

            // Encode the token.
            code = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(code)
                );

            // Tell the world what we are doing.
            _logger.LogDebug("Building the callback url");

            // Build a complete callback endpoint.
            var callbackUrl = string.IsNullOrEmpty(returnUrl) 
                ? $"{uri.Scheme}://{uri.Authority}{_options.Value.Endpoints.ConfirmEmailEndPoint}?userId={userId}&code={code}"
                : $"{uri.Scheme}://{uri.Authority}{_options.Value.Endpoints.ConfirmEmailEndPoint}?userId={userId}&code={code}&returnUrl={returnUrl}";

            // Tell the world what we are doing.
            _logger.LogDebug("Sending the confirmation email");

            // Send the confirmation email.
            await _emailSender.SendEmailAsync(
                email, 
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
                ).ConfigureAwait(false);

            // Should we redirect to the confirmation page?
            if (_signInManager.UserManager.Options.SignIn.RequireConfirmedAccount)
            {
                // Tell the world what we are doing.
                _logger.LogDebug("Creating the redirection endpoint");

                // Build a complete redirect endpoint.
                var redirectUrl = string.IsNullOrEmpty(returnUrl) 
                    ? $"{_options.Value.Endpoints.ConfirmEmailEndPoint}?email={email}" 
                    : $"{_options.Value.Endpoints.ConfirmEmailEndPoint}?email={email}&returnUrl={returnUrl}";

                // Tell the world what we are doing.
                _logger.LogDebug("Redirecting to the confirmation endpoint");

                // Redirect.
                _navigationManager.NavigateTo(redirectUrl);
            }
            else
            {
                // If we get here then we need to redirect to the login endpoint with
                //   our temporary cache key. That way, our middlewear picks up the 
                //   request and performs the logic that requires an HTTP context.

                // Tell the world what we are doing.
                _logger.LogDebug("Creating new key for user: {email}", email);

                // Create a unique key for the user.
                var userKey = $"{Guid.NewGuid()}";

                // Tell the world what we are doing.
                _logger.LogDebug("Caching user: {email}, {key}", email, userKey);
                
                // Create a model for the user.
                var userVM = new CacheVM<TUser>()
                {
                    User = newUser,
                    Password = password,
                    RememberMe = false,
                    LockoutOnFailure = false,
                };

                // Tell the world what we are doing.
                _logger.LogDebug("Caching the VM for the user");

                // Add the model our temporary cache.
                _userCache.AddOrUpdate(
                    userKey,
                    userVM,
                    (key, current) => userVM
                    );

                // Tell the world what we are doing.
                _logger.LogDebug("Calculating redirect");

                // How should we redirect?
                if (string.IsNullOrEmpty(returnUrl))
                {
                    // Redirect with a key and no return url.
                    _navigationManager.NavigateTo(
                        $"{_options.Value.Endpoints.LoginEndPoint}?key={userKey}",
                        true
                        );
                }
                else 
                {
                    // Redirect with a key and a return url.
                    _navigationManager.NavigateTo(
                        $"{_options.Value.Endpoints.LoginEndPoint}?key={userKey}&returnUrl={returnUrl}",
                        true
                        );
                }
            }
        }

        // Return the results.
        return result;
    }

    #endregion
}
