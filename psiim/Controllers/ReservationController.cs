using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        [Authorize]
        public JsonResult Index()
        {
            return new JsonResult("user"+UserId());
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
            return principal?.Claims?.SingleOrDefault(p => p.Type == "UserName")?.Value;
        }
    }
}
