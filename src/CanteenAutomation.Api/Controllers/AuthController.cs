using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System;
using CanteenAutomation.Infrastructure.Persistence.Identity;
using System.Linq;
using CanteenAutomation.Api.Models;
using Serilog;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> userSignInManager, // Corrected parameter name if mismatch was present
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = userSignInManager; // Corrected parameter name if mismatch was present
        _configuration = configuration;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    // [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Determine the role for the new user
        // If a role is provided in the model, use it; otherwise, default to "User"
        string desiredRole = string.IsNullOrEmpty(model.Role) ? "User" : model.Role;

        // Normalize the role name for consistency with Identity's role names
        string normalizedDesiredRole = desiredRole.ToUpperInvariant();

        // // Check if the role exists in the system
        if (!await _roleManager.RoleExistsAsync(desiredRole))
        {
            return BadRequest($"Role '{desiredRole}' does not exist.");
        }

        // Security check: Only Admins can create other Admin or Staff accounts
        // The [Authorize(Roles = "Admin")] attribute handles the "person should also be admin only" part.
        // This additional check ensures that the ADMIN (who is calling this API) can't accidentally
        // // create a role that doesn't exist, or if the Authorization attribute was somehow bypassed.
        // if (!User.IsInRole("Admin") && (normalizedDesiredRole == "ADMIN" || normalizedDesiredRole == "STAFF"))
        // {
        //     // This scenario should ideally be caught by [Authorize(Roles = "Admin")]
        //     // but is here for explicit clarity/redundancy in logic.
        //     return Forbid("Only administrators can create Admin or Staff accounts.");
        // }

        // Prevent non-admin users from setting a role other than "User" if somehow they reached here
        // (e.g., if you later change the Authorize attribute or if an unauthenticated endpoint existed).
        // This enforces that a standard registration (if allowed later) cannot assign privileged roles.
        // if (User.Identity.IsAuthenticated && !User.IsInRole("Admin") && normalizedDesiredRole != "USER")
        // {
        //     return Forbid("Only administrators can create Admin or Staff accounts.");
        // }
        // For unauthenticated requests (if you allow public registration), they can only register as "User".
        // if (!User.Identity.IsAuthenticated && normalizedDesiredRole != "USER")
        // {
        //     return Forbid("Unauthenticated users can only register as a 'User'.");
        // }


        // Check if username already exists
        var existingUser = await _userManager.FindByNameAsync(model.Username);
        if (existingUser != null)
        {
            return Conflict(new { Message = "Username already exists." });
        }

        // Create the user
        var user = new User { UserName = model.Username, Email = model.Email, Role = desiredRole };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Assign the user to the determined role
            await _userManager.AddToRoleAsync(user, desiredRole);
            return Ok(new { Message = $"User registered successfully with role '{desiredRole}'!" });
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized(new { Message = "Invalid username or password." });
        }

        var token = await GenerateJwtToken(user);
        return Ok(new { token = token });
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { Message = "User ID not found in token." });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        Log.Information("User roles: {Roles}", string.Join(", ", roles));

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.Role,
            Roles = roles
        });
    }
    private async Task<string> GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}