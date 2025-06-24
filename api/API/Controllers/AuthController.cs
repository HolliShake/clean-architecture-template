using System.Security.Claims;
using APPLICATION.Dto.Auth;
using AutoMapper;
using APPLICATION.Jwt;
using DOMAIN.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
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
    
    /// <summary>
    /// Login attempt.
    /// </summary>
    /// <returns>AuthData</returns>
    /// <response code="200">When the user is successfully logged in</response>
    /// <response code="400">When the user is not found</response>
    /// <response code="401">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during login</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        if (!user.EmailConfirmed)
        {
            return Unauthorized("Email is not authorized");
        }

        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email);

        var userData = _mapper.Map<AuthDataDto>(user);
        userData.IsGoogle = false;
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;

        return Ok(userData);
    }
    
    /// <summary>
    /// Login attempt via google.
    /// </summary>
    /// <returns>AuthData</returns>
    /// <response code="200">When the user is successfully logged in</response>
    /// <response code="400">When the user is not found</response>
    /// <response code="401">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during login</response>
    [HttpPost("google-signin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GoogleLogin(GoogleDto google)
    {
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
            EmailConfirmed = payload.EmailVerified
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
        {
            goto error;
        }
        
        final:;
        if (!user.EmailConfirmed)
        {
            return Unauthorized("Email is not authorized");
        }
        
        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email);
        
        var userData = _mapper.Map<AuthDataDto>(user);
        userData.IsGoogle = true;
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;
        
        return Ok(userData);
    }
    
    /// <summary>
    /// Register attempt.
    /// </summary>
    /// <returns>Null|Errors</returns>
    /// <response code="204">When the user is successfully registered</response>
    /// <response code="400">When the user is not found</response>
    /// <response code="500">When an unexpected error occurs during registration</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Register(AuthRegisterDto registrationForm)
    {
        var result = await _userManager.CreateAsync(_mapper.Map<User>(registrationForm), registrationForm.Password);

        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    /// <summary>
    /// Generate admin user.
    /// </summary>
    /// <param name="secret">The secret key.</param>
    /// <returns>The admin user.</returns>
    /// <response code="200">When the admin user is successfully generated</response>
    /// <response code="400">When the admin user is not found</response>
    /// <response code="500">When an unexpected error occurs during generation</response>
    [HttpPost("generate/{secret}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GenerateAdmin(string secret)
    {
        var old = await _userManager.FindByEmailAsync(_config["Admin:Email"]);
        var user = new User
        {
            Email = _config["Admin:Email"],
            UserName = _config["Admin:Username"],
            FirstName = _config["Admin:Firstname"],
            LastName = _config["Admin:Lastname"],
            EmailConfirmed = true,
        };
        
        if (!secret.SequenceEqual(_config["Admin:Secret"]))
        {
            goto bad;
        }

        if (old != null)
        {
            // Already exists!
            goto update;
        }

        // Create
        var result = await _userManager.CreateAsync(user, _config["Admin:Password"] ?? "@dmin1234");

        if (result.Succeeded)
        {
            goto ok;
        }

        goto bad;
        
        update:;
        old.EmailConfirmed = old.EmailConfirmed || user.EmailConfirmed;
        var updateResult = await _userManager.UpdateAsync(old);
        if (updateResult.Succeeded)
        {
            goto ok;
        }
        
        bad:;
        return BadRequest("Failed to create admin user.");
        
        ok:;
        return Ok(old ?? user);
    }
    
    /// <summary>
    /// ReAuthenticate every page refresh (must use authentication context in UI).
    /// </summary>
    /// <returns>AuthData|Errors</returns>
    /// <response code="200">When the user is successfully authenticated</response>
    /// <response code="400">When the user is not found</response>
    /// <response code="401">When the user is not authorized</response>
    /// <response code="500">When an unexpected error occurs during authentication</response>
    [HttpGet("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthDataDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Authenticate()
    {
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Replace($"{JwtBearerDefaults.AuthenticationScheme} ", String.Empty);

        if (accessToken.Length <= 0)
        {
            goto bad;
        }
        
        var principal = _jwtAuthManager.DecodeJwtToken(accessToken);
        var userId = principal.Item1.FindFirst(type => type.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if (userId == null)
        {
            goto bad;
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            goto ok;
        }
        
        bad:;
        return NotFound();
        
        ok:;
        var token = JwtGenerator.GenerateToken(_jwtAuthManager, user.Id, user.Email);
        
        var userData = _mapper.Map<AuthDataDto>(user);
        userData.AccessToken = /**/
            token.AccessToken;
        userData.RefreshToken = /**/
            token.RefreshToken.TokenString;
        
        return Ok(userData);
    }
}
