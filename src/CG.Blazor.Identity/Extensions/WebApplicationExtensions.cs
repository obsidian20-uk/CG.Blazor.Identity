
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// This class contains extension methods relates to the <see cref="WebApplicationBuilder"/>
/// type.
/// </summary>
public static partial class WebApplicationExtensions
{
    /// <summary>
    /// This method adds middlewear required for the blazor identity library.
    /// </summary>
    /// <typeparam name="TUser">The type of associated user.</typeparam>
    /// <param name="webApplication">The web application to use for the operation.</param>
    /// <returns>The vlaue of the <paramref name="webApplication"/> parameter,
    /// for chaining calls together, Fluent style.</returns>
    /// <remarks>
    /// <para>
    /// Note, this method must be called, in the <c>Program</c> module, <b>before</b> the 
    /// <see cref="WebApplicationBuilder"/> object is used to creates the <see cref="WebApplication"/> 
    /// instance.
    /// </para>
    /// </remarks>
    public static WebApplication UseBlazorIdentity<TUser>(
        this WebApplication webApplication
        ) where TUser : class
    {
        // Validate the parameter(s) before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplication, nameof(webApplication));

        // Ensure these two methods get called.
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
        
        // Register our custom middlewear module.
        webApplication.UseMiddleware<BlazorIdentityModule<TUser>>();

        // Return the web application.
        return webApplication;
    }
}
