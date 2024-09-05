using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly BlogContext _context;

    public UserRepository(BlogContext context)
    {
        _context = context;
    }

    public User GetUserByUsername(string username)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }
}
