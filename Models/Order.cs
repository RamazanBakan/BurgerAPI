namespace WebApplication10;


public class Order
    {
        public int Id { get; set; } // Sipariş kimliği
        public string CustomerEmail { get; set; } // Müşteri e-posta adresi
        public List<OrderItem> OrderItems { get; set; } // Sipariş öğeleri listesi
        public decimal TotalPrice { get; set; } // Toplam sipariş fiyatı
        public string Status { get; set; } = "Hazırlanıyor"; // Sipariş durumu
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Oluşturulma tarihi
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Güncellenme tarihi
        
      
    }