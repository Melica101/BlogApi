using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly BlogContext _context;

    public PostsController(BlogContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreatePost([FromBody] Post post)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found.");
        }

        // Set the userId and creation date for the post
        post.UserId = int.Parse(userId);
        post.CreatedAt = DateTime.UtcNow;

        // Add the post to the database
        _context.Posts.Add(post);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var posts = await _context
            .Posts.Include(p => p.User)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Body,
                Author = p.User.Username,
                p.CreatedAt,
            })
            .ToListAsync();

        var totalCount = await _context.Posts.CountAsync();

        return Ok(new { totalCount, posts });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(
        int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var post = await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        // Get the total count of comments for this post
        var totalCommentsCount = await _context.Comments.CountAsync(c => c.PostId == id);

        // Paginate the comments
        var comments = await _context
            .Comments.Where(c => c.PostId == id)
            .Include(c => c.User) // Include the user information for each comment
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new
            {
                c.Id,
                c.Body,
                Author = c.User.Username,
                c.CreatedAt,
            })
            .ToListAsync();

        // Return the post and paginated comments
        return Ok(
            new
            {
                post.Id,
                post.Title,
                post.Body,
                Author = post.User.Username,
                post.CreatedAt,
                totalCommentsCount,
                Comments = comments,
            }
        );
    }
}
