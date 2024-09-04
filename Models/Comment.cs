using System;

public class Comment
{
    public int Id { get; set; }             // Primary key
    public string Body { get; set; }         // Comment content
    public int UserId { get; set; }          // Foreign key to the user (commenter)
    public User User { get; set; }           // Navigation property to the User
    public int PostId { get; set; }          // Foreign key to the post
    public Post Post { get; set; }           // Navigation property to the Post
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp for comment creation
}
