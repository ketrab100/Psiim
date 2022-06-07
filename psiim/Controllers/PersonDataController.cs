using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using psiim.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
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
                var p = _context.PeopleData.FirstOrDefault(p => p.Login == peopleDatum.Login);
                if (p != null)
                {
                    var json = Json("User with this login exists");
                    json.StatusCode = 500;
                    return (json);
                }
                var person = _context.PeopleData.Add(peopleDatum);
                client.PersonData = person.Entity;
                _context.Add(client);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
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
            var client = _context.PeopleData.Include(c=>c.Clients).FirstOrDefault(c=>c.PersonDataId == peopleDatum.PersonDataId);
            if (client.Clients.FirstOrDefault().ClientId != userId)
            {
                return Unauthorized();
            }
            else
            {
                client.FirstName = peopleDatum.FirstName;
                client.SecondName = peopleDatum.SecondName;
                client.BirthDate = peopleDatum.BirthDate;
                client.PhoneNumber = peopleDatum.PhoneNumber;
                client.Login = peopleDatum.Login;
                client.HashPassword = peopleDatum.HashPassword;
                try
                {
                    _context.SaveChanges(true);
                }
                catch (Exception e)
                {
                    var json = Json(e.Message);
                    json.StatusCode = 500;
                    return json;
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
        public IActionResult deleteAccount()
        {
            int userId = Int32.Parse(UserId().ToString());
            try
            {
                var client = _context.PeopleData.Include(a=>a.Admins).FirstOrDefault(d => d.PersonDataId == userId);
                _context.PeopleData.Remove(client);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
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
