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
                .OrderByDescending(x => x.posted_date)
                .ToListAsync();


            var replies = await _context.UserPostReplies
                                .OrderByDescending(x => x.replied_on)
                                .ToListAsync();

            // Map replies to each post
            var result = posts.Select(post => new PostReplyModel
            {
                post_id = post.post_id,
                title = post.title,
                author = post.author,
                summary = post.summary,
                user_id = post.user_id,
                posted_date = post.posted_date,
                postprofilepic=post.postprofilepic,
                replies = replies
                    .Where(r => r.post_id == post.post_id)
                    .Select(r => new RepliesModel
                    {
                        reply_id = r.reply_id,
                        user_id = r.user_id,
                        reply_summary = r.reply_summary,
                        replied_on = r.replied_on,
                        post_id = r.post_id,
                        username=r.username,
                        replyprofilepic=r.replyprofilepic
                    })
                    .ToList()
            }).ToList();
            return Ok(result);
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
        public async Task<IActionResult> UpdatePost(int postid, PostsModel Post)
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
                return Ok(new { Message = "Reply Created Successfully!",reply=Reply});
            }
            else
            {
                return BadRequest(new { Message = "Creating a Reply failed!"});
            }
        }



        [HttpDelete("deletereply/{replyid}")]
        public async Task<IActionResult> DeleteReply(int replyid)
        {

            var reply = await _context.UserPostReplies
                                        .FirstOrDefaultAsync(c => c.reply_id == replyid);

            if (reply == null)
            {
                return NotFound(new { message = "There's no reply found." });
            }

            _context.UserPostReplies.Remove(reply);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reply deleted successfully!" });
        }


        [HttpPatch("updatereply/{replyid}")]
        public async Task<IActionResult> UpdateReply(int replyid, RepliesModel Reply)
        {
            
            if (replyid==null)
            {
                return BadRequest(new { message = "Reply ID  is required." });
            }

            if (Reply == null)
            {
                return BadRequest(new { message = "No Reply Information available" });
            }

            // Find the existing company by its organization name
            var existingReply = await _context.UserPostReplies
                                                .FirstOrDefaultAsync(c => c.reply_id == replyid);

            if (existingReply == null)
            {
                return NotFound(new { message = "Reply not found." });
            }
            existingReply.reply_summary = Reply.reply_summary;  
            existingReply.replied_on = Reply.replied_on;  

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Reply updated successfully!",existingReply });
        }

        
        [HttpGet("getleetcodeproblemslist")]
        public async Task<IActionResult> GetLeetcodeProblems()
        {
            var problems = await _context.LeetCodeProblemsData
                .OrderBy(x => x.problem_id)
                .ToListAsync();
            return Ok(problems);
        }

        [HttpPost("postleetcodeproblemslist")]
        public async Task<IActionResult> PostLeetcodeProblems(LeetCodeModel problemData)
        {
             if (ModelState.IsValid)  // Ensure the incoming post is valid
            {
                var maxProblemId = await _context.LeetCodeProblemsData
                .MaxAsync(p => (int?)p.problem_id);  

                problemData.problem_id = maxProblemId.HasValue ? maxProblemId.Value + 1 : 1;
                
                await _context.LeetCodeProblemsData.AddAsync(problemData);  // Add the post asynchronously
                await _context.SaveChangesAsync();  // Save changes to the database
                return Ok(new { Message = "Problem Added Successfully!", Data=problemData });
            }
            else
            {
                return BadRequest(new { Message = "Adding a leetcode problem failed!" });
            }
        }

        [HttpPatch("updateleetcodeproblemslist")]
        public async Task<IActionResult> UpdateLeetcodeProblems(LeetCodeModel problemData)
        {
             if (ModelState.IsValid)  // Ensure the incoming post is valid
            {
                var existingProblem = await _context.LeetCodeProblemsData
                                                .FirstOrDefaultAsync(c => c.problem_id == problemData.problem_id);

                if (existingProblem == null)
                {
                    return NotFound(new { message = "Problem not found." });
                }
                existingProblem.problem_notes = problemData.problem_notes;  
                existingProblem.problem_id = problemData.problem_id;  

                await _context.SaveChangesAsync();

                return Ok(new { message = "Problem updated successfully!",Data= existingProblem });
              }
            else
            {
                return BadRequest(new { Message = "Updating a leetcode problem failed!" });
            }
        }

    }
}