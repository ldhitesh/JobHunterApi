using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResumeController : ControllerBase
    {
        private readonly CompaniesDbContext _context;
        public ResumeController(CompaniesDbContext context)
        {
            _context = context;
        }

        [HttpGet("getresumedata")]
        public async Task<IActionResult> getResumeData([FromQuery]string user_id)
        {
            var resumeData = await _context.ResumeData
                                            .Where(r => r.user_id == user_id) // Filter by user_id (email)
                                            .ToListAsync();
                    
            // If no matching data is found, return a NotFound status
            if (resumeData == null || !resumeData.Any())
            {
                return NotFound("No Resume Data");
            }

            // Return the found resume data
            return Ok(resumeData);
        }

        [HttpPost]
        public async Task<IActionResult> postResumeData(ResumeModel resumedata)
        {
            if (resumedata.details.Length == null || string.IsNullOrWhiteSpace(resumedata.user_id))
                {
                    return BadRequest("Invalid Resume Details");
                }

            var existingResume = await _context.ResumeData.AnyAsync(c => c.user_id == resumedata.user_id);
            if(!existingResume){
                await _context.ResumeData.AddAsync(resumedata);
                await _context.SaveChangesAsync();

                return Ok(new {Message="Resume Added Successfully!"});
            }
            else{
                 var resumedaat = await _context.ResumeData
                                        .FirstOrDefaultAsync(c => c.user_id == resumedata.user_id);

                resumedaat.details=resumedata.details;
                await _context.SaveChangesAsync();
                return BadRequest(new { message = "Resume is already been added!" });
            }
        }

    }
}