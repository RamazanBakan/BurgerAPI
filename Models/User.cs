using System.ComponentModel.DataAnnotations;

namespace WebApplication10;

public class User
{
    [Key] public int Id { get; set; }
    
    public bool IsMember { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool? Gender { get; set; }

    public DateTime? Birthdate { get; set; }

    public string PhoneNumber { get; set; }
    
    public string Email { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
}