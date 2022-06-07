using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using psiim.Models;

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



        [HttpPost("{table}")]
        public IActionResult createTable(Table table)
        {
            try
            {
                _context.Tables.Add(table);
                _context.SaveChanges();
            }
            catch(Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(table);

        }
        //do zmiany 
        [HttpPut("{updatedTable}")]
        public IActionResult updateTable(Table updatedTable)
        {
            try
            {
                _context.Tables.Update(updatedTable);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(updatedTable);
        }
        //???
        [HttpDelete("{table}")]
        public IActionResult deleteTable(Table table)
        {
            try 
            {
                _context.Tables.Remove(table);
                _context.SaveChanges(true);
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
            return new JsonResult(table);
        }

       
    }
}