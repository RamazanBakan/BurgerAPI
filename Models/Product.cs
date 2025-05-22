namespace WebApplication10;

public class Product
{
    public int Id { get; set; } // Ürünün benzersiz kimliği
    public string Name { get; set; } // Ürün adı
    public string Category { get; set; } // Kategori: "Burger", "İçecek", vb.
    public decimal Price { get; set; } // Ürün fiyatı
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Güncellenme tarihi

}