using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Linq;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly BlogDbContext _context;

    public AdminController(BlogDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _context.Users.ToList();
        var blogs = _context.Blogs.Include(b => b.User).ToList();
        
        ViewBag.Users = users;
        ViewBag.Blogs = blogs;
        
        return View();
    }

    [HttpPost]
    public IActionResult DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return NotFound();
        
        _context.Users.Remove(user);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult DeleteBlog(int id)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();
        
        _context.Blogs.Remove(blog);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }
}