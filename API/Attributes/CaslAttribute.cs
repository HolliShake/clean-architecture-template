using System.Net;
using System.Security.Claims;
using System.Text.Json;
using CQI.APPLICATION.Jwt;
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
    private IJwtAuthManager _jwtAuthManager;

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
        
        var accessToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace($"{JwtBearerDefaults.AuthenticationScheme} ", String.Empty);
       
        _jwtAuthManager = (IJwtAuthManager) context.HttpContext.RequestServices.GetService(typeof(IJwtAuthManager));

        if (_jwtAuthManager == null)
        {
            goto bad;
        }

        if (accessToken.Length <= 0)
        {
            goto bad;
        }
        
        var principal = _jwtAuthManager.DecodeJwtToken(accessToken);
        var role = principal.Item1.FindFirst(c => c.Type == ClaimTypes.Role)?.Value;

        Console.WriteLine(">> " + accessToken);
        
        if (role != null)
        {
            var json = JsonSerializer.Deserialize<List<Access>>(role);

            int matches = 0;
            for (var i = 0; i < json!.Count;i++)
            {
                var key = json[i].subject;
                var val = json[i].action;
                var access = $"{key}:{val}";

                if (_validAccessList.Contains(access))
                {
                    ++matches;
                    break;
                }
            }

            if (matches <= 0)
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