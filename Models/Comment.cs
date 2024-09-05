using System;

public class Comment
{
    public int Id { get; set; }            
    public string Body { get; set; }         
    public int UserId { get; set; }          // Foreign key to the user (commenter)
    public User User { get; set; }           // Navigation property to the User
    public int PostId { get; set; }          // Foreign key to the post
    public Post Post { get; set; }           // Navigation property to the Post
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
