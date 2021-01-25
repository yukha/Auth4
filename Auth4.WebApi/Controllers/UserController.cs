using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Auth4.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public UserData Get()
        {
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