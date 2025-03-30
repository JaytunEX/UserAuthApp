using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }  // Table for BlogPost model
    public DbSet<User> Hash { get; set; }

    
}