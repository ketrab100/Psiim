using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using psiim.Models;
using Serilog;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClubController : Controller
    {
        private PSIIMBilardContext _context;
        public ClubController(PSIIMBilardContext pSIIMBilardContext)
        {
            _context = pSIIMBilardContext;
        }

        [HttpGet]
       
        [Produces(typeof(List<Club>))]
        public IActionResult getClubs()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult("Nie masz uprawnień");
            }
            int userId = Int32.Parse(UserId().ToString()); 
            var clubs = _context.Clubs.Where(c=>c.Admins.FirstOrDefault().PersonDataId == userId).ToList();
            return new JsonResult(clubs);
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

    }
}