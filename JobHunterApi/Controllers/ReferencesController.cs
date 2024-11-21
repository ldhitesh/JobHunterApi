using JobHunterApi.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferencesController:ControllerBase
    {
        private readonly CompaniesDbContext _context;
        public ReferencesController(CompaniesDbContext context){
            _context=context;
        }

         // GET: api/references
        [HttpGet]
        public async Task<IActionResult> GetReferences()
        {
            var references = await _context.CompanyReferences.ToListAsync();
            return Ok(references);
        }
    }
}