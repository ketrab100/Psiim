using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        [HttpGet]
        [Produces(typeof(List<Table>))]
        public IActionResult getTables([FromBody]Club club)
        {
            var tables = club.Tables.ToList();
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