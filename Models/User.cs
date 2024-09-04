using System.Collections.Generic;

public class User
{
    public int Id { get; set; }             // Primary key
    public string Username { get; set; }    // Username
    public string PasswordHash { get; set; } // Hashed password
    public ICollection<Post> Posts { get; set; } = new List<Post>(); // User's posts
    public ICollection<Comment> Comments { get; set; } = new List<Comment>(); // User's comments
}
