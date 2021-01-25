using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auth4.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class User2Controller : ControllerBase
    {
        private readonly ILogger<User2Controller> _logger;

        public User2Controller(ILogger<User2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public UserData Get()
        {
            ClaimsPrincipal currentUser = this.User;
            
            return new UserData
            {
                TenantId = "00000000-0000-0000-0000-000000000001",
                UserId = "00000000-0000-0000-0000-000000000002",
                Name = "User Name",
                Roles = new[] {"User", "Developer"}
            };
        }
    }
}