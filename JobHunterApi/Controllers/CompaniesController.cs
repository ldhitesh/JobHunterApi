using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompaniesDbContext _context;

        public CompaniesController(CompaniesDbContext context)
        {
            _context = context;
        }

        // GET: api/companies/getcompaniesnames
        [HttpGet("getcompanyreferences")]
        public async Task<IActionResult> GetCompanyReferences()
        {
          var references = await _context.CompanyReferences
                                .OrderBy(c => c.Organization)  
                                .Select(c => new { c.Organization,c.Name, c.Email })
                                .ToListAsync();
            return Ok(references);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _context.Companies.ToListAsync();
            return Ok(companies);
        }
      
        //POST: api/companies/addcompany
        [HttpPost("addcompany")]
        public async Task<IActionResult> AddCompany([FromBody] CompanyModel company)
        {
            if (company == null || string.IsNullOrWhiteSpace(company.Organization) ||  string.IsNullOrWhiteSpace(company.Description))
            {
                return BadRequest("Invalid Company Details");
            }

            var existingCompany = await _context.Companies.AnyAsync(c => c.Organization == company.Organization);
            if(!existingCompany){
                await _context.Companies.AddAsync(company);
                await _context.SaveChangesAsync();

                return Ok(new {Message="Company Added Successfully!"});
            }
            else{
                return BadRequest(new { message = "Company is already been added!" });
            }
        }


    }
}