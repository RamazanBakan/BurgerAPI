using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication10.DTOS;

namespace WebApplication10.Interface
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(CreateOrderDto orderDto ); // Yeni sipariş oluştur
        
        Task<List<Order>> GetOrdersByEmailAsync(string email); // Email'e göre siparişleri getir

        Task<Order> GetOrderByIdAsync(int id); // Sipariş bilgilerini getir
        
        Task<List<Order>> GetAllOrdersAsync(); // Tüm siparişleri getir
        Task<Order> UpdateOrderStatusAsync(int id, string status); // Sipariş durumunu güncelle
        Task DeleteOrderAsync(int id); // Siparişi sil
        Task<Order> RemoveOrderItemAsync(int orderId, int itemId); // Siparişten ürün çıkar
        Task<Order> AddOrderItemAsync(int orderId, OrderItemDto orderItemDto); // Siparişe ürün ekle
    }
}