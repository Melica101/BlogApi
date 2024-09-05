using System;
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
    private readonly IUnitOfWork _unitOfWork;

    public PostsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        post.UserId = int.Parse(userId);
        post.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Posts.AddPostAsync(post);
        await _unitOfWork.CompleteAsync();

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var posts = await _unitOfWork.Posts.GetPostsAsync(page, pageSize);
        var totalCount = await _unitOfWork.Posts.GetPostsCountAsync();

        var result = posts.Select(p => new
        {
            p.Id,
            p.Title,
            p.Body,
            Author = p.User.Username,
            p.CreatedAt
        });

        return Ok(new { totalCount, posts = result });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var post = await _unitOfWork.Posts.GetPostByIdAsync(id);
        if (post == null) return NotFound();

        var totalCommentsCount = await _unitOfWork.Comments.GetCommentsCountByPostIdAsync(id);
        var comments = await _unitOfWork.Comments.GetCommentsByPostIdAsync(id, page, pageSize);

        return Ok(new
        {
            post.Id,
            post.Title,
            post.Body,
            Author = post.User.Username,
            post.CreatedAt,
            totalCommentsCount,
            Comments = comments.Select(c => new
            {
                c.Id,
                c.Body,
                Author = c.User.Username,
                c.CreatedAt
            })
        });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, [FromBody] Post post)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found.");
        }

        var existingPost = await _unitOfWork.Posts.GetPostByIdAsync(id);
        if (existingPost == null)
        {
            return NotFound("Post not found.");
        }

        // Ensure the user owns the post before allowing an update
        if (existingPost.UserId != int.Parse(userId))
        {
            return Forbid("You are not allowed to update this post.");
        }

        existingPost.Title = post.Title;
        existingPost.Body = post.Body;

        await _unitOfWork.Posts.UpdatePostAsync(existingPost);
        await _unitOfWork.CompleteAsync();

        return Ok(existingPost);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized("User ID not found.");
        }

        var post = await _unitOfWork.Posts.GetPostByIdAsync(id);
        if (post == null)
        {
            return NotFound("Post not found.");
        }

        // Ensure the user owns the post before allowing deletion
        if (post.UserId != int.Parse(userId))
        {
            return Forbid("You are not allowed to delete this post.");
        }

        await _unitOfWork.Posts.DeletePostAsync(post);
        await _unitOfWork.CompleteAsync();

        return Ok("Post deleted successfully.");
    }
}
