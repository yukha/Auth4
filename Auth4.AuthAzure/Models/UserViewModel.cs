using System.Collections.Generic;
using System.Security.Claims;

namespace Auth4.AuthAzure.Models
{
    public class UserViewModel
    {
        public ClaimsPrincipal User { get; set; }
        public IEnumerable<Claim> ParsedClaims { get; set; }
    }
}