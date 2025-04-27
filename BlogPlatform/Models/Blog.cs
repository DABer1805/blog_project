public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Like> Likes { get; set; } = new List<Like>();
}