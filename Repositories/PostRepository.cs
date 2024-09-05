using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class PostRepository : IPostRepository
{
    private readonly BlogContext _context;

    public PostRepository(BlogContext context)
    {
        _context = context;
    }

    public async Task<Post> GetPostByIdAsync(int id)
    {
        return await _context.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize)
    {
        return await _context.Posts.Include(p => p.User)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
    }

    public async Task<int> GetPostsCountAsync()
    {
        return await _context.Posts.CountAsync();
    }

    public async Task AddPostAsync(Post post)
    {
        await _context.Posts.AddAsync(post);
    }

    public Task UpdatePostAsync(Post post)
    {
        _context.Posts.Update(post); 
        return Task.CompletedTask;   
    }

    public Task DeletePostAsync(Post post)
    {
        _context.Posts.Remove(post); 
        return Task.CompletedTask;
    }
}