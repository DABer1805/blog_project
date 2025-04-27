public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; } = "User"; // "Admin" или "User"
    
    public List<Blog> Blogs { get; set; } = new List<Blog>();
    public List<Comment> Comments { get; set; } = new List<Comment>();
    public List<Like> Likes { get; set; } = new List<Like>();
}