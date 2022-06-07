using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using psiim.Models;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace psiim.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TableController : Controller
    {
        private PSIIMBilardContext _context;
        public TableController(PSIIMBilardContext pSIIMBilardContext)
        {
            _context = pSIIMBilardContext;
        }
        /// <summary>
        /// Lista wszystkich stołów w klubie 
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Client")]
        [Route("getTables")]
        public IActionResult getTables(Club club)
        {
            var tables = club.Tables.Where(t=>t.Club == club).ToList();
            return new JsonResult(tables);
        }

        /// <summary>
        /// Lista stołow przypsianych do klubu admina 
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("getMyTables")]
        public IActionResult getMyTables()
        {
            int userId = int.Parse(UserId());
            var user = _context.Admins.FirstOrDefault(u => u.AdminId == userId);
            var clubId = user.ClubId;
            var tables = _context.Tables.Where(t => t.Club.ClubId == clubId).ToList();
            return new JsonResult(tables);
        }
        /// <summary>
        /// Lista stołów wolnych o danej godzinie w konkretnym klubie
        /// </summary>
        /// <param name="clubId"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("getNotReservedTables")]
        public IActionResult getNotReservedTables(int clubId, DateTime dateTime)
        {
            var reservations = _context.Reservations.Where(r => r.Date == dateTime).ToList();
            var reservedTables = new List<Table>();

            foreach(var r in reservations)
            {
                foreach(var rt in r.ReservedTables)
                {
                    reservedTables.Add(rt.Table);
                }
            }
            var tables = _context.Tables.Where(t=>t.ClubId==clubId).ToList().Except(reservedTables);
            
            return new JsonResult(tables);
        }
        /// <summary>
        /// Tworzenie nowego stołu
        /// Obiekt przesyłany w json nie ma w sobie klubu
        /// Klub jest ustawainy na podstawie admina
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        

        [HttpPost]
        [Authorize(Roles="Admin")]
        [Route("createTable")]
        public IActionResult createTable([FromBody]Table table)
        {
            int userId = int.Parse(UserId());
            var user = _context.Admins.Include(u=>u.Club).FirstOrDefault(u => u.AdminId == userId);
            var club = user.Club;
            table.Club = club;
            table.ReservedTables = new List<ReservedTable>();
            try
            {
                _context.Tables.Add(table);
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(table);
        }
        /// <summary>
        /// Edycja stołu
        /// </summary>
        /// <param name="updatedTable"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles ="Admin")]     
        [Route("updateTable")]
        public IActionResult updateTable([FromBody]Table updatedTable)
        {
            int userId = int.Parse(UserId());
            var user = _context.Admins.Include(u => u.Club).FirstOrDefault(u => u.AdminId == userId);
            var club = user.Club;
            if (updatedTable.Club == club)
            {
                var json = Json("You can not edit this table");
                json.StatusCode = 500;
                return json;
            }
            Table table = _context.Tables.FirstOrDefault(t => t.TableId == updatedTable.TableId);
            try
            {
                table.Type = updatedTable.Type;
                table.Number = updatedTable.Number;
                table.Price = updatedTable.Price;
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                var json = Json(e.Message);
                json.StatusCode = 500;
                return json;
            }
            return new JsonResult(updatedTable);
        }
        /// <summary>
        /// Usuwanie stołu jeżeli należy do klubu obsuługiwanego przez danego admina
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        [HttpDelete()]
        [Authorize(Roles ="Admin")]
        [Route("deleteTable")]
        public IActionResult deleteTable([FromBody]Table table)
        {
            int userId = int.Parse(UserId());
            var user = _context.Admins.Include(u => u.Club).FirstOrDefault(u => u.AdminId == userId);
            var club = user.Club;
            if(table.ClubId == club.ClubId)
            {
                try
                {
                    var t = _context.Tables.FirstOrDefault(t => t.TableId == table.TableId);
                    _context.Tables.Remove(t);
                    _context.SaveChanges(true);
                }
                catch (Exception e)
                {
                    var json = Json(e.Message);
                    json.StatusCode = 500;
                    return json;
                }
            }
            else
            {
                var json = Json("You can not delete this table");
                json.StatusCode = 500;
                return json;
            }
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