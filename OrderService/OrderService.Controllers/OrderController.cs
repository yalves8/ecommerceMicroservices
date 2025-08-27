using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.OrderService.Contracts.Events;
using OrderService.OrderService.Domain.Entities;
using OrderService.OrderService.Infrastructure.Data;
using OrderService.OrderService.Messaging;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderDbContext _db;
        private readonly IMessageBusClient _bus;

        public OrderController(OrderDbContext db, IMessageBusClient bus)
        {
            _db = db;
            _bus = bus;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _db.Orders.Include(o => o.Items).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _db.Orders.Include(o => o.Items)
                                        .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order, CancellationToken ct)
        {
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);

            // Cria evento de integração
            var evt = new SalesConfirmedEvent
            {
                OrderId = order.Id,
                Items = order.Items.Select(i => new SalesConfirmedItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Publica no RabbitMQ
            await _bus.PublishSalesConfirmedAsync(evt, ct);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
    }
}
