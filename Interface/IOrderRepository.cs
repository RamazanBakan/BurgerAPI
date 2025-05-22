using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication10.Interface
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id); // Belirli bir siparişi al
        Task<List<Order>> GetAllAsync(); // Tüm siparişleri al
        Task AddAsync(Order order); // Yeni bir sipariş ekle
        Task UpdateAsync(Order order); // Mevcut bir siparişi güncelle
        Task DeleteAsync(int id); // Bir siparişi sil
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId); // Sipariş öğelerini al
    }
}