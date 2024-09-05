using System;
using System.Threading.Tasks;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IPostRepository Posts { get; }
    ICommentRepository Comments { get; }
    Task<int> CompleteAsync();
}
