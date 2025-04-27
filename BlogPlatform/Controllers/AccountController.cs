using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.EntityFrameworkCore;
using System.Linq;

public class AccountController : Controller
{
    private readonly BlogDbContext _context;

    public AccountController(BlogDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            ViewBag.Error = "Неверное имя пользователя или пароль";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            ViewBag.Error = "Пользователь с таким именем уже существует";
            return View();
        }

        _context.Users.Add(new User { Username = username, Password = password });
        _context.SaveChanges();
        
        return RedirectToAction("Login");
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public IActionResult Profile()
    {
        var username = User.Identity.Name;
        var user = _context.Users
            .Include(u => u.Blogs)
            .Include(u => u.Comments)
            .FirstOrDefault(u => u.Username == username);
        
        if (user == null) return RedirectToAction("Login");
        
        return View(user);
    }
}
