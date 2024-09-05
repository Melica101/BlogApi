using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class CommentRepository : ICommentRepository
{
    private readonly BlogContext _context;

    public CommentRepository(BlogContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId, int page, int pageSize)
    {
        return await _context.Comments.Where(c => c.PostId == postId)
                                      .Include(c => c.User)
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();
    }

    public async Task<int> GetCommentsCountByPostIdAsync(int postId)
    {
        return await _context.Comments.CountAsync(c => c.PostId == postId);
    }

    public async Task AddCommentAsync(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
    }
}
