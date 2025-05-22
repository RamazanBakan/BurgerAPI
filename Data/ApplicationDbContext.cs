using Microsoft.EntityFrameworkCore;

using WebApplication10.Models;

//using WebApplication10.Models;

namespace WebApplication10.Data;

public class ApplicationDbContext:DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Product> Products { get; set; } // Ürün tablosu
    public DbSet<Order> Orders { get; set; } // Sipariş tablosu
    public DbSet<OrderItem> OrderItems { get; set; } // Sipariş öğesi tablosu
    
    public DbSet<User> Users { get; set; }// user tablosu
    public DbSet<StoredToken> StoredTokens { get; set; }
    
    public DbSet<Address> Addresses { get; set; } // Address tab
    
}