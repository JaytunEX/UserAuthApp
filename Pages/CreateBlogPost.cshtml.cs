using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;

public class CreateBlogPostModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateBlogPostModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public List<BlogPost> BlogPosts { get; set; } = new List<BlogPost>(); // Lista de blogs

    [BindProperty]
    public BlogPostInputModel Input { get; set; }

    public string Message { get; set; } = "";

    public class BlogPostInputModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }

   public void OnGet()
{
    BlogPosts = _db.BlogPosts.Take(10).ToList(); // Limita a 10 entradas para la prueba
}


    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Retorna la página si hay errores de validación
        }

        // Crear el hash para el blog post
        string hash = GenerateHash(Input.Title, Input.Description);

        var newPost = new BlogPost
        {
            Title = Input.Title,
            Description = Input.Description,
            Hash = hash
        };

        _db.BlogPosts.Add(newPost);
        _db.SaveChanges();

        Message = "Blog post created successfully!";
        return RedirectToPage(); // Recargar la página para mostrar la lista actualizada
    }

    public string ValidateBlogIntegrity(BlogPost post)
    {
        string recomputedHash = GenerateHash(post.Title, post.Description);
        return post.Hash == recomputedHash ? "✅ Valid" : "❌ Altered!";
    }

    private string GenerateHash(string title, string description)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string content = title + description;
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
