using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CG.Blazor.Identity.Middleware;

/// <summary>
/// This class supports our Blazor/Razor based identity operations.
/// </summary>
/// <typeparam name="TUser">The type of associated user.</typeparam>
internal class BlazorIdentityModule<TUser>
   where TUser : class
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

    private EmailChangeModel externalLoginModel;

    private IUserEmailStore<TUser> _emailStore;

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

    public async Task Invoke(HttpContext context, SignInManager<TUser> signInManager, UserManager<TUser> userManager, IUserStore<TUser> userStore, IEmailSender emailSender)
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(context, nameof(context))
            .ThrowIfNull(signInManager, nameof(signInManager));

        _emailStore = GetEmailStore(userManager, userStore);

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
                SignInResult result;

                if (userVM.ExternalLogin)
                {
                    await signInManager.SignInAsync(userVM.User, false);
                    result = SignInResult.Success;
                }
                else
                {

                    // Attempt to log the user in.
                    result = await signInManager.PasswordSignInAsync(
                        userVM.User,
                        userVM.Password,
                        userVM.RememberMe,
                        userVM.LockoutOnFailure
                        );

                    // Once we tried to log in, we no longer need the password.
                    userVM.Password = "";
                }

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


        else if (context.Request.Path.StartsWithSegments(
            _options.Value.Endpoints.ExternalLoginEndPoint
            ) && context.Request.Query.ContainsKey("provider") && context.Request.Query.ContainsKey("returnUrl"))
        {
            var returnUrl = context.Request.Query["returnUrl"];
            var provider = context.Request.Query["provider"];

            var redirectUrl = $"{_options.Value.Endpoints.ExternalLoginCallbackEndPoint}?returnUrl={returnUrl}";


            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            await context.ChallengeAsync(provider, properties);
        }

        else if (context.Request.Path.StartsWithSegments(
            _options.Value.Endpoints.ExternalLoginCallbackEndPoint
            ) && context.Request.Query.ContainsKey("returnUrl"))
        {
            string returnUrl = context.Request.Query["returnUrl"];

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                //ErrorMessage = "Error loading external login information.";
                context.Response.Redirect(_options.Value.Endpoints.LoginEndPoint);
                return;
            }

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                var user = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

                var userKey = BlazorIdentityManager<TUser>.AnnounceLogin(user, "", false, false, true);

                context.Response.Redirect(
                    $"{_options.Value.Endpoints.LoginEndPoint}?key={userKey}",
                    true
                    );
                return;
            }

            if (result.IsLockedOut)
            {
                context.Response.Redirect(returnUrl);
                return;
            }
            else
            {
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    externalLoginModel = new EmailChangeModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                }
                var user = CreateUser();

                await userStore.SetUserNameAsync(user, externalLoginModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, externalLoginModel.Email, CancellationToken.None);

                var createResult = await userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    createResult = await userManager.AddLoginAsync(user, info);
                    if (createResult.Succeeded)
                    {
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        var userId = await userManager.GetUserIdAsync(user);
                        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                        var callbackUrl = $"{_options.Value.Endpoints.ConfirmEmailEndPoint}?userid={userId}&code={code}";

                        await emailSender.SendEmailAsync(externalLoginModel.Email, "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        await signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
                        context.Response.Redirect("/");

                    }
                }
            }
        }

        // By default, call the next module in the pipeline.
        await _next.Invoke(context);
    }

    private TUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<TUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(TUser)}'. " +
                $"Ensure that '{nameof(TUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
        }
    }

    private IUserEmailStore<TUser> GetEmailStore(UserManager<TUser> _userManager, IUserStore<TUser> _userStore)
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<TUser>)_userStore;
    }



    #endregion
}

