using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using UserAuthApp.Helpers; // Make sure you have the EncryptionHelper class

namespace UserAuthApp.Pages
{
  public class RegisterKeyModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public RegisterKeyModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public RegisterInputModel Input { get; set; }

    public string Message { get; set; } = "";
    public string LoginMessage { get; set; } = "";
    public string DecryptedKey { get; set; } = ""; // 🔹 Agregar esta propiedad

    public class RegisterInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Key { get; set; }
    }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string encryptedKey = EncryptionHelper.Encrypt(Input.Key, "hola-javier");

        var newUser = new User
        {
            Username = Input.Username,
            Password = encryptedKey
        };

        _db.Hash.Add(newUser);
        _db.SaveChanges();

        Message = "Registration successful!";
        return Page();
    }

    public IActionResult OnPostLogin()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = _db.Hash.FirstOrDefault(u => u.Username == Input.Username);
        if (user != null)
        {
            string decryptedKey = EncryptionHelper.Decrypt(user.Password, "hola-javier");

            if (decryptedKey == Input.Key)
            {
                HttpContext.Session.SetString("Username", Input.Username);
                DecryptedKey = decryptedKey; // 🔹 Asignar la clave desencriptada

                if (Request.Form["RememberMe"] == "on")
                {
                    Response.Cookies.Append("Username", Input.Username, new CookieOptions { Expires = DateTime.Now.AddDays(30) });
                }

                return Page();
            }
        }

        LoginMessage = "Invalid username or key.";
        return Page();
    }
}

}
