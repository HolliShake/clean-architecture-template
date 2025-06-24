using System.Net;
using System.Security.Claims;
using System.Text.Json;
using APPLICATION.IService;
using APPLICATION.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Attributes;

public class Access
{
    public string subject { get; set; }
    public string action { get; set; }
}

public class CaslAttribute:Attribute, IAuthorizationFilter
{
    private readonly string[] _validAccessList;
    private IJwtAuthManager? _jwtAuthManager;
    private IUserXAccessService? _userXAccessService;

    public CaslAttribute(params string[] accessList)
    {
        _validAccessList = accessList;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (_validAccessList.Length <= 0)
        {
            return;
        }
        
        var accessToken = context.HttpContext.Request.Headers.Authorization
            .ToString()
            .Replace($"{JwtBearerDefaults.AuthenticationScheme} ", String.Empty);

        

        _jwtAuthManager ??= (IJwtAuthManager?) context.HttpContext.RequestServices.GetService(typeof(IJwtAuthManager));

        _userXAccessService ??= (IUserXAccessService?) context.HttpContext.RequestServices.GetService(typeof(IUserXAccessService));

        // If the jwtAuthManager is not found, return unauthorized
        if (_jwtAuthManager == null || _userXAccessService == null)
        {
            goto bad;
        }

        if (string.IsNullOrEmpty(accessToken))
        {
            goto bad;
        }
        
        var principal = _jwtAuthManager.DecodeJwtToken(accessToken);
        var currentUserId = principal.Item1.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if (!string.IsNullOrEmpty(currentUserId))
        {
            if (!_userXAccessService.UserHasAccess(currentUserId, _validAccessList))
            {
                goto bad;
            }

            return;
        }
        
        bad:;
        context.Result = new UnauthorizedResult();
        context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
        context.HttpContext.Response.CompleteAsync();
        return;
    }
}