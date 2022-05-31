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
    public class PersonDataController : Controller
    {
        private PSIIMBilardContext _context;
        public PersonDataController(PSIIMBilardContext pSIIMBilardContext)
        {
            _context = pSIIMBilardContext;
        }

        [HttpGet]
        public IActionResult getAccountData(int id)
        {
            PeopleDatum user = new PeopleDatum();
            string role = "";
            try
            {
                user = _context.PeopleData.FirstOrDefault(p => p.PersonDataId == id);
                var user_role = _context.Admins.FirstOrDefault(p => p.PersonDataId == id);
                role = "";
                if (user_role == null)
                {
                    role = "Client";
                }
                else
                {
                    role = "Admin";
                }

            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(new { role = role, user = user });
        }
        [HttpPost]
        [Route("create")]
        public IActionResult createClientAccount(Client client)
        {
            try
            {
                _context.Clients.Add(client);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(client);

        }
        [HttpPost]
        [Route("edit")]
        public IActionResult editAccount(PeopleDatum peopleDatum)
        {
            try
            {
                _context.PeopleData.Update(peopleDatum);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(peopleDatum);

        }
        [HttpDelete]
        public IActionResult deleteAccount(PeopleDatum peopleDatum)
        {
            try
            {
                _context.PeopleData.Remove(peopleDatum);
                var user_role = _context.Admins.FirstOrDefault(p => p.PersonDataId == peopleDatum.PersonDataId);
                //if (user_role == null)
                //{
                //    _context.Clients.Remove(peopleDatum.Clients);
                //}
                //else
                //{
                //    _context.Admins.Remove(peopleDatum.Admins);
                //}
                //_context.SaveChanges(true);

            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(peopleDatum);

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
