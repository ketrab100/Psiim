using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using psiim.Models;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : Controller
    {
        public IConfiguration _configuration;
        private PSIIMBilardContext _context;

        public TokenController(IConfiguration config, PSIIMBilardContext pSIIMBilardContext)
        {
            _configuration = config;
            _context = pSIIMBilardContext;
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
                if (_context.Admins.FirstOrDefault(a=>a.PersonData == user) != null)
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
                    expires: DateTime.UtcNow.AddMinutes(20),
                    signingCredentials: signIn);

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
        }
    }
}
