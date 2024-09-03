public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string Body { get; set; }
    public Post Post { get; set; }
}
