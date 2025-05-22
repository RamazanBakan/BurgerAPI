using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication10.DTOS;
using WebApplication10.Interface;
using WebApplication10.Service; // RabbitMqPublisher burada tanımlı olmalı

namespace WebApplication10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
       // private readonly RabbitMqPublisher _rabbitMqPublisher;
        
        public OrderController(IOrderService orderService/*RabbitMqPublisher rabbitMqPublisher*/)
        {
            _orderService = orderService;
           /*_rabbitMqPublisher = rabbitMqPublisher; */        }

        /// <summary>
        /// Tüm siparişleri getir
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// ID'ye göre siparişi getir
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        [HttpGet("by-email")]
        public async Task<IActionResult> GetOrdersByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email adresi belirtilmelidir.");
            }

            try
            {
                var orders = await _orderService.GetOrdersByEmailAsync(email);
                if (!orders.Any())
                {
                    return NotFound("Bu e-posta adresine ait sipariş bulunamadı.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        /// <summary>
        /// Yeni sipariş oluştur
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            if (orderDto == null || orderDto.OrderItems == null || orderDto.OrderItems.Count == 0)
            {
                return BadRequest("Sipariş bilgileri eksik.");
            }

            try
            {
                // 📌 Siparişi veritabanına kaydet
                var order = await _orderService.CreateOrderAsync(orderDto);

                // 📌 Siparişi RabbitMQ kuyruğuna gönder
             //   _rabbitMqPublisher.PublishOrderToQueue(order); // ✅ Doğru metod çağrısı

                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
        /// <summary>
        /// Sipariş durumunu güncelle
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Sipariş durumu boş olamaz.");
            }

            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(id, status);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        /// <summary>
        /// Siparişi sil
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);
                return Ok(new { message = $"Order with ID {id} has been deleted." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }

        /// <summary>
        /// Siparişe ürün ekle
        /// </summary>
        [HttpPost("{id}/items")]
        public async Task<IActionResult> AddOrderItem(int id, [FromBody] OrderItemDto orderItemDto)
        {
            if (orderItemDto == null || orderItemDto.ProductId <= 0 || orderItemDto.Quantity <= 0)
            {
                return BadRequest("Ürün bilgileri eksik veya geçersiz.");
            }

            try
            {
                var updatedOrder = await _orderService.AddOrderItemAsync(id, orderItemDto);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
        

        /// <summary>
        /// Siparişten ürün çıkar
        /// </summary>
        [HttpDelete("{id}/items/{itemId}")]
        public async Task<IActionResult> RemoveOrderItem(int id, int itemId)
        {
            try
            {
                var updatedOrder = await _orderService.RemoveOrderItemAsync(id, itemId);
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { message = $"Bir hata oluştu: {ex.Message}" });
            }
        }
    }
}
