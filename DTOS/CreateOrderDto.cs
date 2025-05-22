using WebApplication10.Models;

namespace WebApplication10.DTOS;

public class CreateOrderDto
{
    public string CustomerEmail { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}
