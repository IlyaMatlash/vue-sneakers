using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myApi.Data;
using myApi.Models;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<object>> CreateOrder(OrderDto orderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверка наличия элементов заказа
            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            {
                return BadRequest(new { message = "Заказ должен содержать хотя бы один товар" });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Создание заказа
                    var order = new Order
                    {
                        TotalPrice = orderDto.TotalPrice,
                        RecipientName = orderDto.RecipientName,
                        RecipientAddress = orderDto.RecipientAddress,
                        PostalCode = orderDto.PostalCode,
                        PhoneNumber = orderDto.PhoneNumber,
                        Email = orderDto.Email,
                        PromoCode = orderDto.PromoCode,
                        DeliveryMethod = orderDto.DeliveryMethod,
                        OrderDate = orderDto.OrderDate != default ? orderDto.OrderDate : DateTime.UtcNow,
                        Status = "New"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Создание элементов заказа
                    foreach (var itemDto in orderDto.OrderItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            Price = itemDto.Price,
                            Name = itemDto.Name
                        };

                        _context.OrderItems.Add(orderItem);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // Возвращаем информацию о созданном заказе
                    return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, new
                    {
                        id = order.OrderId,
                        message = "Заказ успешно создан",
                        itemsCount = orderDto.OrderItems.Count
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { message = "Произошла ошибка при создании заказа", error = ex.Message });
                }
            }
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }
    }
}