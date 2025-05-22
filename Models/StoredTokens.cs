using System.ComponentModel.DataAnnotations;

namespace WebApplication10.Models;

public class StoredToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public string UserName { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Password { get; set; }
}