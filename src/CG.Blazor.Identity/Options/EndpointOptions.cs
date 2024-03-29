﻿
namespace CG.Blazor.Identity.Options;

/// <summary>
/// This class contains configuration settings related to endpoint overrides, 
/// for Blazor identity operations.
/// </summary>
public class EndpointOptions
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.LoginEndPoint"/> property.
    /// </summary>
    private string _loginEndPoint = Constants.LoginEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.RegisterEndPoint"/> property.
    /// </summary>
    private string _registerEndPoint = Constants.RegisterEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.ResetEndPoint"/> property.
    /// </summary>
    private string _resetEndPoint = Constants.ResetEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.ProfileEndPoint"/> property.
    /// </summary>
    private string _profileEndPoint = Constants.ProfileEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.LogoutEndPoint"/> property.
    /// </summary>
    private string _logoutEndPoint = Constants.LogoutEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions.ConfirmEmailEndPoint"/> property.
    /// </summary>
    private string _confirmEmailEndPoint = Constants.ConfirmEmailEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions._externalLoginEndPoint"/> property.
    /// </summary>
    private string _externalLoginEndPoint = Constants.ExternalLoginEndPoint;

    /// <summary>
    /// The field backs the <see cref="EndpointOptions._externalLoginCallbackEndPoint"/> property.
    /// </summary>
    private string _externalLoginCallbackEndPoint = Constants.ExternalLoginCallbackEndPoint;

    #endregion

    // *******************************************************************
    // Properties.
    // *******************************************************************

    #region Properties

    /// <summary>
    /// This property contains an optional endpoint for login operations.
    /// </summary>
    [Required]
    public string LoginEndPoint 
    {
        get { return _loginEndPoint; } 
        set
        {
            // Is the value populated?
            if (!string.IsNullOrEmpty(value))
            {
                // Assign the value.
                _loginEndPoint = value.StartsWith('/') 
                    ? value 
                    : $"/{value}";
            }
            else
            {
                // Assign the default.
                _loginEndPoint = Constants.LoginEndPoint;
            }
        }
    }

    /// <summary>
    /// This property contains an optional endpoint for registration operations.
    /// </summary>
    [Required]
    public string RegisterEndPoint
    {
        get { return _registerEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_registerEndPoint.StartsWith('/'))
            {
                _registerEndPoint = $"/{value}";
            }
            else
            {
                _registerEndPoint = value;
            }
        }
    }

    /// <summary>
    /// This property contains an optional endpoint for password reset operations.
    /// </summary>
    [Required]
    public string ResetEndPoint
    {
        get { return _resetEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_resetEndPoint.StartsWith('/'))
            {
                _resetEndPoint = $"/{value}";
            }
            else
            {
                _resetEndPoint = value;
            }
        }
    }

    /// <summary>
    /// This property contains an optional endpoint for profile operations.
    /// </summary>
    [Required]
    public string ProfileEndPoint
    {
        get { return _profileEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_profileEndPoint.StartsWith('/'))
            {
                _profileEndPoint = $"/{value}";
            }
            else
            {
                _profileEndPoint = value;
            }
        }
    }

    /// <summary>
    /// This property contains an optional endpoint for logout operations.
    /// </summary>
    [Required]
    public string LogoutEndPoint
    {
        get { return _logoutEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_logoutEndPoint.StartsWith('/'))
            {
                _logoutEndPoint = $"/{value}";
            }
            else
            {
                _logoutEndPoint = value;
            }
        }
    }

    /// <summary>
    /// This property contains an optional endpoint for confirm email operations.
    /// </summary>
    [Required]
    public string ConfirmEmailEndPoint
    {
        get { return _confirmEmailEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_confirmEmailEndPoint.StartsWith('/'))
            {
                _confirmEmailEndPoint = $"/{value}";
            }
            else
            {
                _confirmEmailEndPoint = value;
            }
        }
    }


    /// <summary>
    /// This property contains an optional endpoint for confirm email operations.
    /// </summary>
    [Required]
    public string ExternalLoginEndPoint
    {
        get { return _externalLoginEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_externalLoginEndPoint.StartsWith('/'))
            {
                _externalLoginEndPoint = $"/{value}";
            }
            else
            {
                _externalLoginEndPoint = value;
            }
        }
    }

    [Required]
    public string ExternalLoginCallbackEndPoint
    {
        get { return _externalLoginCallbackEndPoint; }
        set
        {
            // Should we add a leading '/' character?
            if (!_externalLoginCallbackEndPoint.StartsWith('/'))
            {
                _externalLoginCallbackEndPoint = $"/{value}";
            }
            else
            {
                _externalLoginCallbackEndPoint = value;
            }
        }
    }

    #endregion

}
