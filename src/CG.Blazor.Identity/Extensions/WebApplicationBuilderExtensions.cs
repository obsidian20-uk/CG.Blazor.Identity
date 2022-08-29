
using CG;
using CG.Blazor.Identity.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// This class contains extension methods relates to the <see cref="WebApplicationBuilder"/>
/// type.
/// </summary>
/// <remarks>
/// <para>
/// Note, this method must be called, in the <c>Program</c> module, <b>before</b> the call to 
/// <c>UseRouting</c>, or this library simply won't work.
/// </para>
/// </remarks>
public static partial class WebApplicationBuilderExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <param name="configuration">The configuration section from which 
    /// to read blazor identity options.</param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity(
        this WebApplicationBuilder webApplicationBuilder,
        IConfiguration configuration
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Call the overload.
        return webApplicationBuilder.AddBlazorIdentity<IdentityUser>(
            configuration
            );
    }

    // *******************************************************************

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity(
        this WebApplicationBuilder webApplicationBuilder
        )
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Call the overload.
        return webApplicationBuilder.AddBlazorIdentity<IdentityUser>(
            (Action<BlazorIdentityOptions>?)null,
            (Action<IdentityOptions>?)null
            );
    }

    // *******************************************************************

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <typeparam name="TUser">The type of associated user.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <param name="configuration">The configuration section from which 
    /// to read blazor identity options.</param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity<TUser>(
        this WebApplicationBuilder webApplicationBuilder,
        IConfiguration configuration
        ) where TUser : IdentityUser
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Call the overload.
        return webApplicationBuilder.AddBlazorIdentity<TUser>(
            configuration,
            (Action<IdentityOptions>?)null
            );
    }

    // *******************************************************************

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <typeparam name="TUser">The type of associated user.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity<TUser>(
        this WebApplicationBuilder webApplicationBuilder
        ) where TUser : IdentityUser
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Call the overload.
        return webApplicationBuilder.AddBlazorIdentity<TUser>(
            (Action<BlazorIdentityOptions>?)null,
            (Action<IdentityOptions>?)null
            );
    }

    // *******************************************************************

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <typeparam name="TUser">The type of associated user.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <param name="configuration">The configuration section from which 
    /// to read blazor identity options.</param>
    /// <param name="configureIdentityOptions">An optional delegate 
    /// for configuring the ASP.NET identity options. </param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity<TUser>(
        this WebApplicationBuilder webApplicationBuilder,
        IConfiguration configuration,
        Action<IdentityOptions>? configureIdentityOptions = null
        ) where TUser : IdentityUser
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder))
            .ThrowIfNull(configuration, nameof(configuration));

        // Bind the options to the configuration.
        var blazorIdentityOptions = new BlazorIdentityOptions();
        configuration.Bind(blazorIdentityOptions);

        // Call the overload.
        return webApplicationBuilder.AddBlazorIdentity<TUser>(
            x => blazorIdentityOptions.CopyTo(x),
            configureIdentityOptions
            );
    }

    // *******************************************************************

    /// <summary>
    /// This method registers the types required to support Blazor/Razor
    /// based identity operations.
    /// </summary>
    /// <typeparam name="TUser">The type of associated user.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder
    /// to use for the operation.</param>
    /// <param name="configureBlazorIdentityOptions">An optional delegate 
    /// for configuring the blazor identity options.</param>
    /// <param name="configureIdentityOptions">An optional delegate 
    /// for configuring the ASP.NET identity options. </param>
    /// <returns>An <see cref="IdentityBuilder"/> instance that may be 
    /// used to further configure identity operations.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more of the arguments is missing, or invalid.</exception>
    public static IdentityBuilder AddBlazorIdentity<TUser>(
        this WebApplicationBuilder webApplicationBuilder,
        Action<BlazorIdentityOptions>? configureBlazorIdentityOptions = null,
        Action<IdentityOptions>? configureIdentityOptions = null
        ) where TUser : IdentityUser
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder));

        // Add the old school ASP.NET identity libraries.
        var identityBuilder = webApplicationBuilder.Services.AddDefaultIdentity<TUser>(identityOptions =>
        {
            if (null == configureIdentityOptions)
            {
                // Create default options.
                identityOptions.SignIn.RequireConfirmedAccount = true;
            }
            else
            {
                // Give the caller a chance to change the options.
                configureIdentityOptions.Invoke(identityOptions);
            }
        });

        // Give the caller a chance to specify options.
        var blazorIdentityOptions = new BlazorIdentityOptions();
        configureBlazorIdentityOptions?.Invoke(blazorIdentityOptions);

        // Creat a validation context.
        var context = new ValidationContext(
            blazorIdentityOptions,
            serviceProvider: null,
            items: null
            );

        // Validate the options.
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(
            blazorIdentityOptions,
            context,
            validationResults,
            true
            );

        // Did we fail?
        if (!isValid)
        {
            // Panic!!
            throw new ArgumentException(
                message: string.Join(
                    ',',
                    validationResults.Select(x => x.ErrorMessage
                    ))
                );
        }

        // Register the options as a service.
        webApplicationBuilder.Services.AddSingleton<IOptions<BlazorIdentityOptions>>(
            new OptionsWrapper<BlazorIdentityOptions>(blazorIdentityOptions)
            );

        // Register the services we'll need.
        webApplicationBuilder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<TUser>>();
        webApplicationBuilder.Services.AddHttpContextAccessor();
        webApplicationBuilder.Services.AddScoped<BlazorIdentityManager<TUser>>();

        // Return the identity builder.
        return identityBuilder;
    }

    #endregion
}
