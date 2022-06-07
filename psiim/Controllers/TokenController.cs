using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using psiim.Models;
using Microsoft.Extensions.Caching.Distributed;
using Serilog;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : Controller
    {
        public IConfiguration _configuration;
        private PSIIMBilardContext _context;
        private IDistributedCache _cache;
        private IHttpContextAccessor _httpContextAccessor;

        public TokenController(IConfiguration config, PSIIMBilardContext pSIIMBilardContext, IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = config;
            _context = pSIIMBilardContext;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        public async Task<IActionResult> Post(string login, string password)
        {
            var user = _context.PeopleData.Where(u => u.Login == login && u.HashPassword == password).FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                Claim[] claims = { };
                if (_context.Admins.FirstOrDefault(a => a.PersonData == user) != null)
                {
                    claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.PersonDataId.ToString()),
                        new Claim("UserName", user.Login),
                        new Claim(ClaimTypes.Role, "Admin")
                    };
                }
                if (_context.Clients.FirstOrDefault(c => c.PersonData == user) != null)
                {
                    claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.PersonDataId.ToString()),
                        new Claim("UserName", user.Login),
                        new Claim(ClaimTypes.Role, "Client")
                    };
                }


                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
        }
        protected string UserId()
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                foreach (var claim in principal.Claims)
                {
                    Log.Debug($"CLAIM TYPE: {claim.Type}; CLAIM VALUE: {claim.Value}");
                }

            }
            return principal?.Claims?.SingleOrDefault(p => p.Type == "Id")?.Value;
        }
        protected string UserLogin()
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                foreach (var claim in principal.Claims)
                {
                    Log.Debug($"CLAIM TYPE: {claim.Type}; CLAIM VALUE: {claim.Value}");
                }

            }
            return principal?.Claims?.SingleOrDefault(p => p.Type == "UserName")?.Value;
        }
        protected string UserRole()
        {
            var principal = HttpContext.User;
            if (principal?.Claims != null)
            {
                foreach (var claim in principal.Claims)
                {
                    Log.Debug($"CLAIM TYPE: {claim.Type}; CLAIM VALUE: {claim.Value}");
                }

            }
            return principal?.Claims?.SingleOrDefault(p => p.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
        }

    }
}
