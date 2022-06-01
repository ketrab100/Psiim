using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using psiim.Models;
using Microsoft.EntityFrameworkCore;

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
            return principal?.Claims?.SingleOrDefault(p => p.Type == "UserName")?.Value;
        }
        //[HttpGet]
        //public IActionResult GetReservations()
        //{
        //    var reservations = _context.Reservations.ToList();
        //    return new JsonResult(reservations);
           
        //}

        //[HttpGet("{id}")]
        //[Produces(typeof(Reservation))]
        //[Route("getReservationById")]
        //public IActionResult GetReservationById([FromRoute] int id)
        //{
        //    var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId.Equals(id));
        //    if(reservation==null) return NotFound();
        //    return new JsonResult(reservation);
        //}
        [HttpPost("{reservation}")]
        public IActionResult CreateReservation([FromBody] Reservation reservation)
        {
            try
            {
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
            }
            catch(Exception e)
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
        /// <summary>
        /// Akceptacja rezerwacji przez admina
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        [HttpPost("acceptReservation/{reservation}")]
        [Authorize(Roles = "Admin")]
        //[Route("acceptReservation")]
        public IActionResult acceptReservation(Reservation reservation)
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
        [HttpPost("denyReservation/{reservation}")]
        [Authorize(Roles = "Admin")]
        //[Route("denyReservation")]
        public IActionResult denyReservation(Reservation reservation)
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
