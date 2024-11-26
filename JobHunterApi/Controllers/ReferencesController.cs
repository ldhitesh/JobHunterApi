using JobHunterApi.Database;
using JobHunterApi.Models;
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

         //POST: api/references/addreference
        [HttpPost("addreference")]
        public async Task<IActionResult> AddReference([FromBody] ReferencesModel reference)
        {
            if (reference == null ||    string.IsNullOrWhiteSpace(reference.Name) || 
                                        string.IsNullOrWhiteSpace(reference.Organization) ||
                                        string.IsNullOrWhiteSpace(reference.Position) ||
                                        string.IsNullOrWhiteSpace(reference.Email))
            {
                return BadRequest("Invalid Referer Details");
            }

            var existingreference = await _context.CompanyReferences.AnyAsync(c => c.Email == reference.Email);
            if(!existingreference){
                await _context.CompanyReferences.AddAsync(reference);
                await _context.SaveChangesAsync();

                return Ok(new {Message="Reference Added Successfully!"});
            }
            else{
                return BadRequest(new { message = "Reference already been added!" });
            }
        }

        [HttpDelete("deletereference/{email}")]
        public async Task<IActionResult> DeleteCompany(string email)
        {
            var references = await _context.CompanyReferences
                                        .FirstOrDefaultAsync(c => c.Email == email);

            if (references == null)
            {
                return NotFound(new { message = "Referer not found." });
            }

            // Remove the company from the database
            _context.CompanyReferences.Remove(references);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Referer removed successfully!" });
        }

        [HttpPatch("updatereference/{email}")]
        public async Task<IActionResult> UpdateCompany(string email, [FromBody] ReferencesModel references)
        {
            
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new { message = "Referer Email is required." });
            }

            if (references == null || 
                string.IsNullOrWhiteSpace(references.Organization) || 
                string.IsNullOrWhiteSpace(references.Name)||
                string.IsNullOrWhiteSpace(references.Position)||
                string.IsNullOrWhiteSpace(references.Email))
            {
                return BadRequest(new { message = "All the details are required." });
            }

            // Find the existing reference by its email name
            var existingreference = await _context.CompanyReferences
                                                .FirstOrDefaultAsync(c => c.Email == email);

            if (existingreference == null)
            {
                return NotFound(new { message = "Referer not found." });
            }
            existingreference.Organization = references.Organization;  // Update other fields as needed
            existingreference.Name = references.Name;  // Update other fields as needed
            existingreference.Position = references.Position;  // Update other fields as needed
            existingreference.Email = references.Email;  // Update other fields as needed

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Company updated successfully!" });
        }

    }

    
}