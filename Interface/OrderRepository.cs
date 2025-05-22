using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication10.Data;
using WebApplication10.Interface;

namespace WebApplication10.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Belirli bir siparişi getir
        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) // Sipariş öğeleriyle birlikte ürünleri dahil et
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Tüm siparişleri getir
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product) // Sipariş öğeleriyle birlikte ürünleri dahil et
                .ToListAsync();
        }

        // Yeni bir sipariş ekle
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        // Mevcut bir siparişi güncelle
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // Bir siparişi sil
        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        // Sipariş öğelerini getir
        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product) // Sipariş öğeleriyle birlikte ürünleri dahil et
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }
    }
}
