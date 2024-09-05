using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICommentRepository
{
    Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId, int page, int pageSize);
    Task<int> GetCommentsCountByPostIdAsync(int postId);
    Task AddCommentAsync(Comment comment);
}
