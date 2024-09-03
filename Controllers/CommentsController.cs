using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/posts/{postId}/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly BlogContext _context;

    public CommentsController(BlogContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Comment>>> GetComments(int postId)
    {
        return await _context.Comments.Where(c => c.PostId == postId).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Comment>> CreateComment(int postId, Comment comment)
    {
        comment.PostId = postId;
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetComments), new { postId = comment.PostId }, comment);
    }
}
