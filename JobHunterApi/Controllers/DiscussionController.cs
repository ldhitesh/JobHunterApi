using JobHunterApi.Database;
using JobHunterApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace JobHunterApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController:ControllerBase
    {
        private readonly DiscussionDBContext _context;
        public DiscussionController(DiscussionDBContext context)
        {
            _context = context;
        }
        
        [HttpGet("getposts")]
        public async Task<IActionResult> GetPosts()
        {
          var posts = await _context.UserPosts
                                .ToListAsync();
            return Ok(posts);
        }


        [HttpGet("getreplies")]
        public async Task<IActionResult> GetReplies()
        {
          var replies = await _context.UserPostReplies
                                .ToListAsync();
            return Ok(replies);
        }
    }
}