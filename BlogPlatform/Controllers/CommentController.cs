using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize]
public class CommentController : Controller
{
    private readonly BlogDbContext _context;

    public CommentController(BlogDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Add(int blogId, string text)
    {
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Login", "Account");
        
        _context.Comments.Add(new Comment 
        { 
            Text = text, 
            BlogId = blogId, 
            UserId = user.Id 
        });
        _context.SaveChanges();
        
        return RedirectToAction("Details", "Blog", new { id = blogId });
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
        if (comment == null) return NotFound();
        
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null || (user.Role != "Admin" && comment.UserId != user.Id))
        {
            return Forbid();
        }
        
        var blogId = comment.BlogId;
        _context.Comments.Remove(comment);
        _context.SaveChanges();
        
        return RedirectToAction("Details", "Blog", new { id = blogId });
    }
}