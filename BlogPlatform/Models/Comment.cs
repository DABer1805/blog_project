public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}