using APPLICATION.Dto.Auth;
using AutoMapper;
using CQI.APPLICATION.Jwt;
using DOMAIN.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController:ControllerBase
{
    private readonly ConfigurationManager _config;
    private readonly IMapper _mapper;
    private readonly IJwtAuthManager _jwtAuthManager;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    
    public AuthController(ConfigurationManager config, IMapper mapper, IJwtAuthManager jwtAuthManager, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _config = config;
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

        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email, user.Role);

        var userData = _mapper.Map<AuthDataDto>(user);
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;

        return Ok(userData);
    }
    
    [HttpPost("/[controller]/google-signin")]
    public async Task<ActionResult> GoogleLogin(GoogleDto google)
    {
        Console.WriteLine(google.GToken);
        
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            // Change this to your google client ID
            Audience = new List<string>() { _config["Auth:Google:ClientId"] }
        };

        var payload  = GoogleJsonWebSignature.ValidateAsync(google.GToken, settings).Result;

        if (payload == null)
        {
            goto error;
        }

        if (payload.Email != null)
        {
            goto ok;
        }
        
        error:;
        return BadRequest("Invalid google signin!");
        
        ok:;
        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user != null)
        {
            goto final;
        }
        /****** Create new user *****/
        user = new User
        {
            Email = payload.Email,
            UserName = payload.Email,
            FirstName = payload.GivenName,
            LastName = payload.FamilyName,
            EmailConfirmed = payload.EmailVerified,
            Role = "[{ \"subject\": \"auth\", \"action\": \"read\" }]"
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            goto error;
        }
        
        final:;
        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email, user.Role);
        
        var userData = _mapper.Map<AuthDataDto>(user);
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;
        
        return Ok(userData);
    }
    
    [HttpPost("/[controller]/register")]
    public async Task<ActionResult> Register(AuthRegisterDto registrationForm)
    {
        var result = await _userManager.CreateAsync(_mapper.Map<User>(registrationForm), registrationForm.Password);

        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }
    
    [Authorize]
    [HttpGet("/[controller]/authenticate")]
    public async Task<ActionResult> Authenticate()
    {
        var accessToken = Request.Headers[HeaderNames.Authorization];
        return Ok(accessToken);
    }
}