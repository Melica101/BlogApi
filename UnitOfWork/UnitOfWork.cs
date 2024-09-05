using System.Threading.Tasks;

public class UnitOfWork : IUnitOfWork
{
    private readonly BlogContext _context;

    public UnitOfWork(BlogContext context, IUserRepository userRepository, IPostRepository postRepository, ICommentRepository commentRepository)
    {
        _context = context;
        Users = userRepository;
        Posts = postRepository;
        Comments = commentRepository;
    }

    public IUserRepository Users { get; private set; }
    public IPostRepository Posts { get; private set; }
    public ICommentRepository Comments { get; private set; }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
