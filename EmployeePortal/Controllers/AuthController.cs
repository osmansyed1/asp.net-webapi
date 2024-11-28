using Azure;
using EmployeePortal.Interface;
using EmployeePortal.Models;
using EmployeePortal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClient;

        /*  private readonly IHttpClientFactory _httpClientFactory;
private readonly IConfiguration _configuration;*/

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService,
            HttpClient httpClient, RoleManager<IdentityRole> roleManager

            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _httpClient = httpClient;
            _roleManager = roleManager;
        }

        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] RoleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if (roleExists)
            {
                return BadRequest($"Role '{model.Name}' already exists.");
            }

            var role = new IdentityRole { Name = model.Name };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = $"Role '{model.Name}' created successfully." });
        }


        [HttpGet("GetRoles")]
        public async Task<ActionResult<IEnumerable<IdentityRole>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }


        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new IdentityUser { UserName = registerDto.Username, Email = registerDto.Email };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(registerDto.Role))
                {
                    var roleExists = await _roleManager.RoleExistsAsync(registerDto.Role);
                    if (roleExists)
                    {
                        await _userManager.AddToRoleAsync(user, registerDto.Role);
                    }
                    else
                    {
                        return BadRequest($"Role '{registerDto.Role}' does not exist.");
                    }
                }

                return Ok(new { Message = "User registered successfully." });
            }

            return BadRequest(result.Errors);
        }


      

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var token =  _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }
            return Unauthorized();
        }


        // This is the new Azure AD authentication method
        [HttpPost("LoginWithAzureAd")]
        [Authorize] // Requires a valid Azure AD token
        public async Task<IActionResult> LoginWithAzureAd()
        {
            // The user is authenticated via Azure AD if the request has a valid Azure AD token
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }


            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token});
        }

        [HttpPost("LoginWithFacebook")]
        public async Task<ActionResult> LoginWithFacebook([FromBody] FacebookLoginDto facebookLoginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate the access token with Facebook
            var validationResponse = await ValidateAccessToken(facebookLoginDto.AccessToken);
            if (validationResponse == null || !validationResponse.IsValid)
            {
                return Unauthorized("Invalid access token.");
            }

            // Check if the user already exists
            var user = await _userManager.FindByEmailAsync(validationResponse.Email);
            if (user == null)
            {
                // If user doesn't exist, create a new one
                user = new IdentityUser { UserName = validationResponse.Name, Email = validationResponse.Email };
                await _userManager.CreateAsync(user);
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
       

        private async Task<FacebookValidationResponse> ValidateAccessToken(string accessToken)
        {
            var requestUri = $"https://graph.facebook.com/me?access_token={accessToken}&fields=id,name,email";
            var response = await _httpClient.GetStringAsync(requestUri);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<FacebookValidationResponse>(response)!;
        }




      /*  [HttpGet("LoginWithAzureAD")]
        [Authorize(AuthenticationSchemes = "AzureAD")]
        public async Task<IActionResult> LoginWithAzureAD()
        {
            var azureAdUser = User;
            var email = azureAdUser.FindFirst(ClaimTypes.Email)?.Value;
            var name = azureAdUser.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest("Email claim not found in Azure AD token");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = name ?? email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [HttpGet("UserInfo")]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            return Ok(new
            {
                Username = User.Identity?.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
            });
        }*/



    }

}