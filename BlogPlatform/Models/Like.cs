public class Like
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}