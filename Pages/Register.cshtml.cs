using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

public class RegisterModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public RegisterModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; }

    public string Message { get; set; } = "";
    public string LoginMessage { get; set; } = "";

    // Register input model
    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Hash the password before saving (bcrypt instead of SHA256)
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(Input.Password);

        var newUser = new User
        {
            Username = Input.Username,
            Password = hashedPassword
        };

        _db.Hash.Add(newUser);
        _db.SaveChanges();

        Message = "Registration successful!";
        return Page();
    }

    // Login logic
    public IActionResult OnPostLogin()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Check if the user exists
        var user = _db.Hash.FirstOrDefault(u => u.Username == Input.Username);
        if (user != null && BCrypt.Net.BCrypt.Verify(Input.Password, user.Password))
        {
            // Password is correct, create a session
            HttpContext.Session.SetString("Username", Input.Username);

            // If RememberMe is checked, save a cookie
            if (Request.Form["RememberMe"] == "on")
            {
                Response.Cookies.Append("Username", Input.Username, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
            }

            return RedirectToPage("/Blog"); // Redirect to blog or dashboard after login
        }

        // If login fails
        LoginMessage = "Invalid username or password.";
        return Page();
    }
}
