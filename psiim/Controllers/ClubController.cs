using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using psiim.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;

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
        /// <summary>
        /// Zwraca klub do którego adminem jest aktualnie zalogowany użytkownik
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getClub")]
        [Authorize(Roles ="Admin")]
        public IActionResult getClub()
        {
            int userId = Int32.Parse(UserId().ToString()); 
            var clubs = _context.Clubs.Include(t=>t.Tables).FirstOrDefault(c=>c.Admins.FirstOrDefault().PersonDataId == userId);
            return new JsonResult(clubs);
        }
        /// <summary>
        /// Zwraca listę wszytskich klubów w systemie
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("clubsList")]
        [AllowAnonymous]
        public IActionResult clubsList()
        {
            var clubs = _context.Clubs.ToList();
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