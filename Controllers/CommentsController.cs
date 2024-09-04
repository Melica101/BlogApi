using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/posts/{postId}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly BlogContext _context;

    public CommentsController(BlogContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddComment(int postId, [FromBody] Comment comment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var post = await _context.Posts.FindAsync(postId);
        if (post == null)
        {
            return NotFound();
        }

        comment.UserId = int.Parse(userId);
        comment.PostId = postId;
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int postId)
    {
        var post = await _context.Posts
            .Include(p => p.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null) return NotFound();

        var comments = post.Comments.Select(c => new
        {
            c.Id,
            c.Body,
            Author = c.User.Username,
            c.CreatedAt
        });

        return Ok(comments);
    }
}
