using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using psiim.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ReservationController : Controller
    {
        private PSIIMBilardContext _context;
        public ReservationController(PSIIMBilardContext pSIIMBilardContext)
        {
            _context = pSIIMBilardContext;
        }

        /// <summary>
        /// Zwraca rezeracje
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("getReservations")]
        public IActionResult GetReservations()
        {
            var reservations = _context.Reservations.ToList();
            return new JsonResult(reservations);
        }


        /// <summary>
        /// Tworzenie rezerwacji na konkretny stół danego dnia
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Client")]
        [Route("createReservation")]
        public IActionResult CreateReservation([FromBody] DateReservation dateReservation)
        {
            //dynamic deserialized = JObject.Parse(Convert.ToString(data));
            //dynamic table = JObject.Parse(Convert.ToString(deserialized.table));
            //DateTime dateTime = deserialized.dataTime;
            int userId = Int32.Parse(UserId().ToString());
            Reservation reservation = new Reservation(userId, dateReservation.Date, 15.99, false, 1, _context.Clients.FirstOrDefault(c => c.ClientId == userId));
            try
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(reservation);

        }
        /// <summary>
        /// Lista rezerwacji klienta
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Client")]
        [Route("clientReservations")]
        public IActionResult ClientReservations()
        {
            int userId = Int32.Parse(UserId().ToString());
            var client = _context.Clients.Include(c => c.Reservations).FirstOrDefault(c => c.ClientId == userId);
            if (client == null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(client.Reservations);
            }
        }
        /// <summary>
        /// Usunięcie rezerwacji przez klienta
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpDelete]
        [Authorize(Roles = "Client")]
        [Route("deleteReservation")]
        public IActionResult DeleteReservation([FromBody] Reservation reservation)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.ReservationId == reservation.ReservationId);
            if (res == null)
            {
                var json = Json("Reservation does not exist");
                json.StatusCode = 500;
                return json;
            }
            try
            {
                _context.Reservations.Remove(res);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(res);
        }
        /// <summary>
        /// Zwraca niezaakceptowane rezerwacje 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getUnacceptedReservations")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUnacceptedReservations()
        {
            var unaccepted = new List<Reservation>();
            int userId = Int32.Parse(UserId().ToString());
            var reservations = _context.Reservations.ToList();

            foreach (var reservation in reservations)
            {
                if (reservation.IsAccepted == false)
                {
                    unaccepted.Add(reservation);
                }
            }

            return new JsonResult(unaccepted);
        }


        /// <summary>
        /// Akceptacja rezerwacji przez admina
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("acceptReservation")]
        public IActionResult acceptReservation([FromBody] Reservation reservation)
        {
            try
            {
                reservation.IsAccepted = true;
                _context.Reservations.Update(reservation);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(reservation);
        }
        /// <summary>
        /// Odrzucenie rezerwacji przez admina
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("denyReservation")]
        public IActionResult denyReservation([FromBody] Reservation reservation)
        {
            try
            {
                reservation.IsAccepted = false;
                _context.Reservations.Update(reservation);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(reservation);
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
