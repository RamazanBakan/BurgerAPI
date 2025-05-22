using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;
using WebApplication10.DTOS;
using WebApplication10.Interface;

namespace WebApplication10.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(
            IOrderRepository orderRepository,
            ApplicationDbContext context,
            EmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Yeni bir sipariş oluştur
        public async Task<Order> CreateOrderAsync(CreateOrderDto orderDto)
        {
            decimal totalPrice = 0;
            var orderItems = new List<OrderItem>();

            // Sipariş öğelerini oluştur
            foreach (var item in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {item.ProductId} not found.");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    TotalPrice = product.Price * item.Quantity
                };

                totalPrice += orderItem.TotalPrice;
                orderItems.Add(orderItem);
            }

            // Siparişi oluştur
            var order = new Order
            {
                CustomerEmail = orderDto.CustomerEmail,
                OrderItems = orderItems,
                TotalPrice = totalPrice,
                Status = "Hazırlanıyor",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);

            // Sipariş oluşturulduğunda e-posta bildirimi gönder
            string emailBody = $"Merhaba, siparişiniz başarıyla oluşturuldu!\nToplam tutar: {totalPrice}₺.\nSipariş Durumu: {order.Status}";
            await _emailService.SendEmailAsync(orderDto.CustomerEmail, "Sipariş Onayı", emailBody);

            return order;
        }
        
        public async Task<List<Order>> GetOrdersByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email boş olamaz.", nameof(email));
            }

            return await _context.Orders
                .Include(o => o.OrderItems) // Sipariş öğelerini de dahil et
                .Where(o => o.CustomerEmail == email)
                .ToListAsync();
        }


        // Belirli bir siparişi getir
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found.");
            }
            return order;
        }

        // Tüm siparişleri getir
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        // Sipariş durumunu güncelle
        public async Task<Order> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found.");
            }

            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            // Sipariş durumu güncellendiğinde e-posta bildirimi gönder
            string emailBody = $"Merhaba, siparişinizin durumu güncellendi: {status}.";
            await _emailService.SendEmailAsync(order.CustomerEmail, "Sipariş Durumu Güncellendi", emailBody);

            return order;
        }

        // Siparişe ürün ekle
        public async Task<Order> AddOrderItemAsync(int orderId, OrderItemDto orderItemDto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var product = await _context.Products.FindAsync(orderItemDto.ProductId);
            if (product == null)
            {
                throw new Exception($"Product with ID {orderItemDto.ProductId} not found.");
            }

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = orderItemDto.Quantity,
                TotalPrice = product.Price * orderItemDto.Quantity
            };

            order.OrderItems.Add(orderItem);
            order.TotalPrice += orderItem.TotalPrice;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            return order;
        }

        // Siparişten ürün çıkar
        public async Task<Order> RemoveOrderItemAsync(int orderId, int itemId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new Exception($"Order with ID {orderId} not found.");
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.Id == itemId);
            if (orderItem == null)
            {
                throw new Exception($"Order item with ID {itemId} not found in Order {orderId}.");
            }

            order.OrderItems.Remove(orderItem);
            order.TotalPrice -= orderItem.TotalPrice;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            return order;
        }

        // Siparişi sil
        public async Task DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new Exception($"Order with ID {id} not found.");
            }

            await _orderRepository.DeleteAsync(id);
        }
    }
}
