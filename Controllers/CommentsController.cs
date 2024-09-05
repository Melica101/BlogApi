using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/posts/{postId}/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CommentsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        var post = await _unitOfWork.Posts.GetPostByIdAsync(postId);
        if (post == null)
        {
            return NotFound();
        }

        comment.UserId = int.Parse(userId);
        comment.PostId = postId;

        await _unitOfWork.Comments.AddCommentAsync(comment);
        await _unitOfWork.CompleteAsync();

        return Ok(comment);
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var comments = await _unitOfWork.Comments.GetCommentsByPostIdAsync(postId, page, pageSize);
        var totalCommentsCount = await _unitOfWork.Comments.GetCommentsCountByPostIdAsync(postId);

        return Ok(new { totalCommentsCount, Comments = comments });
    }
}
