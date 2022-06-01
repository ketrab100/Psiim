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
        /// <summary>
        /// Zwraca informacje o kliencie  
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getAccountData")]
        [Authorize(Roles = "Client")]
        public IActionResult getAccountData()
        {
            int userId = Int32.Parse(UserId().ToString());
            PeopleDatum user = new PeopleDatum();
            try
            {
                user = _context.PeopleData.FirstOrDefault(p => p.PersonDataId == userId);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(user);
        }
        /// <summary>
        /// Tworzenie nowego clienta
        /// </summary>
        /// <param name="peopleDatum"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createClientAccount")]
        [AllowAnonymous]
        public IActionResult createClientAccount(PeopleDatum peopleDatum)
        {
            Client client = new Client();
            try
            {
                var person = _context.PeopleData.Add(peopleDatum);
                client.PersonData = person.Entity;
                _context.Add(client);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new JsonResult(client);

        }
        /// <summary>
        /// Edycja danych personalnych klienta
        /// Jeżeli id clienta nie jest zgodne z danymi personalnymi w paramatrze zwraca Unauthorized
        /// </summary>
        /// <param name="peopleDatum"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("edit")]
        [Authorize(Roles = "Client")]
        public IActionResult editAccount(PeopleDatum peopleDatum)
        {
            int userId = Int32.Parse(UserId().ToString());
            var client = _context.Clients.FirstOrDefault(c=>c.PersonData == peopleDatum);
            if (client.ClientId != userId)
            {
                return Unauthorized();
            }
            else
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
        }
        /// <summary>
        /// Usuwanie konta klienta
        /// </summary>
        /// <param name="peopleDatum"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("deleteAccount")]
        [Authorize(Roles ="Client")]
        public IActionResult deleteAccount(PeopleDatum peopleDatum)
        {
            try
            {
                _context.PeopleData.Remove(peopleDatum);
                var user_role = _context.Admins.FirstOrDefault(p => p.PersonDataId == peopleDatum.PersonDataId);
                if (user_role == null)
                {
                    _context.Clients.Remove(peopleDatum.Clients.FirstOrDefault());
                }
                else
                {
                    _context.Admins.Remove(peopleDatum.Admins.FirstOrDefault());
                }
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            };
            return new OkResult();
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
