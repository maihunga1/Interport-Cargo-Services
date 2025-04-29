using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

namespace UserController;
public class UserController : Controller
{
    private readonly DbContext.ApplicationDbContext _context;

    public UserController(DbContext.ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("register")]
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        return View();
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        return View(user);
    }

    [Route("login")]
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

        if (user != null)
        {
            // Set authentication cookie here
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError("", "Invalid login attempt.");
        return View();
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("UserAuth");
        return RedirectToAction("Index", "Home");
    }
}