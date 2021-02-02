using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auth4.AuthAzure.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Auth4.AuthAzure.Controllers
{
    public class AuthController : Controller
    {
        private readonly AzureAdSettings _azureAdSettings;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IOptions<AzureAdSettings> azureAdSettings)
        {
            _logger = logger;
            _azureAdSettings = azureAdSettings.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            var location = _azureAdSettings.AzureLoginUrl;
            location += $"?client_id={_azureAdSettings.ClientId}";
            location += $"&redirect_uri={_azureAdSettings.BaseUrl}/Auth/Signed";
            location += "&response_type=id_token";
            location += "&scope=openid profile";
            location += "&response_mode=form_post";
            location += "&nonce=123"; // TODO set real nonce 

            return Redirect(location);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Signed([FromForm] string id_token, [FromForm] string session_state)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(id_token);

            // return View("Test", new UserViewModel{ User = User, ParsedClaims = jwt.Claims});
            // TODO: check nonce
            var claims = new List<Claim>
            {
                new("tid", jwt.Claims.FirstOrDefault(c => c.Type == "tid")?.Value ?? string.Empty),
                new("sub", jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty),
                new("username",
                    jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                    jwt.Claims.First(c => c.Type == "preferred_username")?.Value ?? string.Empty),
                new("roles", "Developer") // jwt.Claims.FirstOrDefault("roles") - vraci array
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc =
                    DateTimeOffset.UtcNow.AddMinutes(10), // TODO jwt.Claims.FirstOrDefault(c => c.Type == "exp")?.Value
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/sign-in");
        }

        private List<Claim> FillClaims()
        {
            var claims = new List<Claim>
            {
                new("tid",
                    User.FindFirstValue("tid") ??
                    User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid") ?? ""),
                new("sub", User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")),
                new("username", User.FindFirstValue("name") ?? User.Identity?.Name ?? ""),
                new("roles", "Developer TODO get-it-from-db")
            };
            return claims;
        }

        public IActionResult Logout()
        {
            return Redirect(
                $"{_azureAdSettings.AzureLogoutUrl}?post_logout_redirect_uri={_azureAdSettings.BaseUrl}/Auth/SignedOut");
        }

        public IActionResult SignedOut()
        {
            HttpContext.SignOutAsync();
            return Redirect("/signed-out");
        }


        [Authorize]
        public IActionResult Test()
        {
            var claims = FillClaims();
            return View(new UserViewModel {User = User, ParsedClaims = claims});
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}