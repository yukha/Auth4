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
                TenantId = User.Claims.FirstOrDefault(c => c.Type == "tid")?.Value,
                UserId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value,
                Name = User.Claims.FirstOrDefault(c => c.Type == "username")?.Value,
                Roles = User.Claims.FirstOrDefault(c => c.Type == "roles")?.Value.Split(' '),
            };
        }
    }
}