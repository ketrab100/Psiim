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
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("getReservations")]
        public IActionResult GetReservations()
        {
            var reservations = _context.Reservations.ToList();
            return new JsonResult(reservations);
            //[HttpGet]
            //public IActionResult GetReservations()
            //{
            //    var reservations = _context.Reservations.ToList();
            //    return new JsonResult(reservations);

        //[HttpGet]
        //public IActionResult GetReservations()
        //{
        //    var reservations = _context.Reservations.ToList();
        //    return new JsonResult(reservations);
           
        }

        [HttpGet("{id}")]
        public IActionResult GetReservationById([FromRoute] int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId.Equals(id));
            if(reservation==null) return NotFound();
            return new JsonResult(reservation);
        }
        /// <summary>
        /// Tworzenie rezerwacji na konkretny stół danego dnia
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Client")]
        [Route("createReservation")]
        public IActionResult CreateReservation([FromBody] dynamic data)
        {
            dynamic deserialized = JObject.Parse(Convert.ToString(data));
            dynamic table = JObject.Parse(Convert.ToString(deserialized.table));
            DateTime dateTime = deserialized.dataTime;
            int userId = Int32.Parse(UserId().ToString());
            Reservation reservation = new Reservation(userId, dateTime, 15.99, false, 1, _context.Clients.FirstOrDefault(c => c.ClientId == userId));
            try
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(reservation);
           
        }

        [HttpPut("{updatedReservation}")]
        public IActionResult UpdateReservation([FromBody] Reservation updatedReservation)
        {
            try
            {
                _context.Reservations.Update(updatedReservation);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(updatedReservation);
        }
        /// <summary>
        /// Lista rezerwacji klienta
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet("getClientReservations/{id}")]
        [Authorize(Roles = "Client")]
        //[Route("")]
        public IActionResult GetClientReservations([FromRoute] int clientId)
        {
            var client = _context.Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (client == null) return NotFound();
           // List<Reservation> reservations = (List<Reservation>) client.Reservations;
            else
            {
                return new JsonResult(client.Reservations);
            }
        }
        /// <summary>
        /// Usunięcie rezerwacji przez klienta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Client")]
        //[Route("")]
        public IActionResult DeleteReservation([FromRoute] int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId.Equals(id));
            if(reservation==null) return NotFound();
            try 
            {
                _context.Reservations.Remove(reservation);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(reservation);
        }
        [HttpGet]
        [Route("getUnacceptedReservations")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUnacceptedReservations()
        {
            var unaccepted = new List<Reservation>();
            //int userId = Int32.Parse(UserId().ToString());
            var reservations = _context.Reservations.Include(r=>r.Client).ToList();
            //return new JsonResult(reservations);
            //try
            //{   
            //    var admin = _context.Admins.FirstOrDefault(a => a.AdminId == userId);
            //    var club = _context.Clubs.Include(t => t.Tables).Where(c => c.Admins.Contains(admin)).FirstOrDefault();
            //    var tables = club.Tables;
            //    var reservations = new List<Reservation>();
            //    foreach (var table in tables)
            //    {
            //        var reservedTables = table.ReservedTables;
            //        foreach(var reservedTable in reservedTables)
            //        {
            //            reservations.Add(reservedTable.Reservation);
            //        }
            //    }


            foreach (var reservation in reservations)
                {
                    if(reservation.IsAccepted == false)
                    {
                        unaccepted.Add(reservation);
                    }
                }
               
            //}
            //catch(Exception e)
            //{
            //    return new JsonResult(e);
            //}
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
            catch(Exception e)
            {
                return new JsonResult(e);
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
        public IActionResult denyReservation([FromBody]Reservation reservation)
        {
            try
            {
                reservation.IsAccepted = false;
                _context.Reservations.Update(reservation);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(reservation);
        }
    }
    
}
