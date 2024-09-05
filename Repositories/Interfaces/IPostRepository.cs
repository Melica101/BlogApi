using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPostRepository
{
    Task<Post> GetPostByIdAsync(int id);
    Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize);
    Task<int> GetPostsCountAsync();
    Task AddPostAsync(Post post);
}
