using System.Collections.Generic;

public class User
{
    public int Id { get; set; }             
    public string Username { get; set; }    
    public string PasswordHash { get; set; } 
    public ICollection<Post> Posts { get; set; } = new List<Post>(); // User's posts
    public ICollection<Comment> Comments { get; set; } = new List<Comment>(); // User's comments
}
