using System.Threading.Tasks;

public interface IUserRepository
{
    User GetUserByUsername(string username);
    Task<User> GetUserByIdAsync(int userId);
    Task AddUserAsync(User user);
}
