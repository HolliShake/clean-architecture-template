using APPLICATION.Dto.Auth;
using APPLICATION.IService;
using AutoMapper;
using CQI.APPLICATION.Jwt;
using DOMAIN.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace API.Controllers;

public class AuthController:ControllerBase
{
    private readonly IMapper _mapper;
    private readonly JwtAuthManager _jwtAuthManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    
    public AuthController(IMapper mapper, JwtAuthManager jwtAuthManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _mapper = mapper;
        _jwtAuthManager = jwtAuthManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("/[controller]/login")]
    public async Task<ActionResult> LoginAttempt(AuthDto credential)
    {
        var user = await _userManager.FindByEmailAsync(credential.Email);

        if (user == null)
        {
            goto Bad;
        }

        var attempt = await _signInManager.CheckPasswordSignInAsync(user, credential.Password, false);

        if (attempt.Succeeded)
        {
            goto Ok;
        }

        Bad:;
        // Set as alternative to Not Found to prevent brute forcing...
        return BadRequest("Invalid username or password");

        Ok:;
        // If allow email verification
        // if (!user.EmailConfirmed)
        // {
        //     return Unauthorized("Email is not authorized");
        // }

        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email, "[]");

        var userData = _mapper.Map<AuthDataDto>(user);
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;

        return Ok(userData);
    }
    
    [Authorize]
    [HttpGet("/[controller]/authenticate")]
    public async Task<ActionResult> Authenticate()
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        return Ok(accessToken);
    }
}