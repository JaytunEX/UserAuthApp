using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public int ID { get; set; }

    [Required, MaxLength(50)]
    public string Username { get; set; }

    [Required, MaxLength(255)]
    public string Password { get; set; }
}
