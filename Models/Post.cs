using System;
using System.Collections.Generic;

public class Post
{
    public int Id { get; set; }              
    public string Title { get; set; }         
    public string Body { get; set; }          
    public int UserId { get; set; }           // Foreign key to the user (author)
    public User User { get; set; }            // Navigation property to the User
    public ICollection<Comment> Comments { get; set; } = new List<Comment>(); // Related comments
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

