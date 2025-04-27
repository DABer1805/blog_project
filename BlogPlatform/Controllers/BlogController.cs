using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class BlogController : Controller
{
    private readonly BlogDbContext _context;

    public BlogController(BlogDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var blogs = _context.Blogs.Include(b => b.User).Include(b => b.Comments).Include(b => b.Likes).ToList();
        return View(blogs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string title, string content)
    {
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Login", "Account");

        _context.Blogs.Add(new Blog 
        { 
            Title = title, 
            Content = content, 
            UserId = user.Id 
        });
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var blog = _context.Blogs
            .Include(b => b.User)
            .Include(b => b.Comments)
                .ThenInclude(c => c.User)
            .Include(b => b.Likes)
                .ThenInclude(l => l.User)
            .FirstOrDefault(b => b.Id == id);
            
        if (blog == null) return NotFound();
        
        return View(blog);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();
        
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null || (user.Role != "Admin" && blog.UserId != user.Id))
        {
            return Forbid();
        }
        
        _context.Blogs.Remove(blog);
        _context.SaveChanges();
        
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Like(int id)
    {
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Login", "Account");
        
        var existingLike = _context.Likes.FirstOrDefault(l => l.BlogId == id && l.UserId == user.Id);
        if (existingLike != null)
        {
            _context.Likes.Remove(existingLike);
        }
        else
        {
            _context.Likes.Add(new Like { BlogId = id, UserId = user.Id });
        }
        
        _context.SaveChanges();
        
        return RedirectToAction("Details", new { id = id });
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return RedirectToAction("Index");
        }

        var blogs = _context.Blogs
            .Include(b => b.User)
            .Include(b => b.Comments)
            .Include(b => b.Likes)
            .Where(b => b.Title.Contains(query) || b.Content.Contains(query))
            .ToList();
            
        ViewBag.SearchQuery = query;
        return View(blogs);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Edit(int id)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();
        
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null || (user.Role != "Admin" && blog.UserId != user.Id))
        {
            return Forbid();
        }
        
        return View(blog);
    }

    [HttpPost]
    [Authorize]
    public IActionResult Edit(int id, string title, string content)
    {
        var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
        if (blog == null) return NotFound();
        
        var username = User.Identity.Name;
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null || (user.Role != "Admin" && blog.UserId != user.Id))
        {
            return Forbid();
        }
        
        blog.Title = title;
        blog.Content = content;
        _context.SaveChanges();
        
        return RedirectToAction("Details", new { id = blog.Id });
    }
}

