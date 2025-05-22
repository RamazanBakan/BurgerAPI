using System;

namespace WebApplication10.Models
{
    public class Address
    {
        public int Id { get; set; } // Adresin kimliği
        public int UserId { get; set; } // Kullanıcı kimliği (ilişkilendirme için)
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // User ile ilişki (istenirse)
        public User User { get; set; }
    }
}