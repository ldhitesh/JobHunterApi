using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Authorization;
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

        // [Authorize(AuthenticationSchemes = "Bearer" ,Policy = "AdminPolicy")]
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

        [HttpDelete("deletecompany/{organization}")]
        public async Task<IActionResult> DeleteCompany(string organization)
        {
            if (string.IsNullOrWhiteSpace(organization))
            {
                return BadRequest(new { message = "Organization name is required." });
            }

            var company = await _context.Companies
                                        .FirstOrDefaultAsync(c => c.Organization == organization);

            if (company == null)
            {
                return NotFound(new { message = "Company not found." });
            }

            // Remove the company from the database
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Company deleted successfully!" });
        }

        [HttpPatch("updatecompany/{organization}")]
        public async Task<IActionResult> UpdateCompany(string organization, [FromBody] CompanyModel company)
        {
            
            if (string.IsNullOrWhiteSpace(organization))
            {
                return BadRequest(new { message = "Organization name is required." });
            }

            if (company == null || 
                string.IsNullOrWhiteSpace(company.Organization) || 
                string.IsNullOrWhiteSpace(company.Description))
            {
                return BadRequest(new { message = "Organization and Description are required." });
            }

            // Find the existing company by its organization name
            var existingCompany = await _context.Companies
                                                .FirstOrDefaultAsync(c => c.Organization == organization);

            if (existingCompany == null)
            {
                return NotFound(new { message = "Company not found." });
            }
            existingCompany.Organization = company.Organization;  // Update other fields as needed
            existingCompany.Description = company.Description;  // Update other fields as needed
            existingCompany.LastApplied = company.LastApplied;  // Update other fields as needed
            existingCompany.Status = company.Status;  // Update other fields as needed
            existingCompany.Link = company.Link;  // Update other fields as needed

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Company updated successfully!",company });
        }




    }
}