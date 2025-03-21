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
          var posts = await _context.UserPosts.OrderByDescending(x => x.posted_date)
                                .ToListAsync();
            return Ok(posts);
        }

        [HttpPost("addpost")]
        public async Task<IActionResult> CreatePost(PostsModel Post)
        {
            if (ModelState.IsValid)  // Ensure the incoming post is valid
            {
                await _context.UserPosts.AddAsync(Post);  // Add the post asynchronously
                await _context.SaveChangesAsync();  // Save changes to the database
                return Ok(new { Message = "Post Created Successfully!" });
            }
            else
            {
                return BadRequest(new { Message = "Creating a post failed!" });
            }
        }


        [HttpDelete("deletepost/{postid}")]
        public async Task<IActionResult> DeletePost(int postid)
        {

            var post = await _context.UserPosts
                                        .FirstOrDefaultAsync(c => c.post_id == postid);

            if (post == null)
            {
                return NotFound(new { message = "There's no post found." });
            }

            // Remove the company from the database
            _context.UserPosts.Remove(post);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post deleted successfully!" });
        }


        [HttpPatch("updatepost/{postid}")]
        public async Task<IActionResult> UpdateCompany(int postid, PostsModel Post)
        {
            
            if (postid==null)
            {
                return BadRequest(new { message = "Post ID  is required." });
            }

            if (Post == null)
            {
                return BadRequest(new { message = "No Post Information available" });
            }

            // Find the existing company by its organization name
            var existingPost = await _context.UserPosts
                                                .FirstOrDefaultAsync(c => c.post_id == postid);

            if (existingPost == null)
            {
                return NotFound(new { message = "Post not found." });
            }
            existingPost.title = Post.title;  
            existingPost.summary = Post.summary;  
            existingPost.posted_date = Post.posted_date;  

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Post updated successfully!",Post });
        }




        [HttpGet("getreplies")]
        public async Task<IActionResult> GetReplies()
        {
          var replies = await _context.UserPostReplies
                                .ToListAsync();
            return Ok(replies);
        }

        [HttpPost("addreply")]
        public async Task<IActionResult> AddReply(RepliesModel Reply)
        {
            if (ModelState.IsValid)  // Ensure the incoming post is valid
            {
                await _context.UserPostReplies.AddAsync(Reply);  // Add the post asynchronously
                await _context.SaveChangesAsync();  // Save changes to the database
                return Ok(new { Message = "Reply Created Successfully!" });
            }
            else
            {
                return BadRequest(new { Message = "Creating a Reply failed!" });
            }
        }
    }
}