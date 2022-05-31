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
        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations = _context.Reservations.ToList();
            return new JsonResult(reservations);
           
        }

        [HttpGet("{id}")]
        [Produces(typeof(Reservation))]
        public IActionResult GetReservationById([FromRoute] int id)
        {
            var reservation = _context.Reservations.FirstOrDefault(r => r.ReservationId.Equals(id));
            if(reservation==null) return NotFound();
            return new JsonResult(reservation);
        }
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

        [HttpDelete("{id}")]
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
        [HttpPost("reservation")]
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
    }
    
}
