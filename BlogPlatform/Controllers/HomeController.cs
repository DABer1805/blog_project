using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using System.Linq;

public class HomeController : Controller
{
    private readonly BlogDbContext _context;

    public HomeController(BlogDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var blogs = _context.Blogs
            .Include(b => b.User)
            .Include(b => b.Comments)
            .Include(b => b.Likes)
            .OrderByDescending(b => b.CreatedAt)
            .ToList();
            
        return View(blogs);
    }
}