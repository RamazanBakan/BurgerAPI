namespace WebApplication10;

public class OrderItem
{
    public int Id { get; set; } // Sipariş öğesi kimliği
    public int OrderId { get; set; } // Sipariş kimliği (bağlantı)
    public Order Order { get; set; } // Sipariş bağlantısı
    public int ProductId { get; set; } // Ürün kimliği (bağlantı)
    public Product Product { get; set; } // Ürün bağlantısı
    public int Quantity { get; set; } // Ürün miktarı
    public decimal TotalPrice { get; set; } // Bu öğenin toplam fiyatı
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Güncellenme tarihi
}