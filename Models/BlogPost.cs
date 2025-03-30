public class BlogPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = "";  // Default to an empty string if not provided

    public string Hash { get; set; } // This is to store the hash value for integrity validation
}
