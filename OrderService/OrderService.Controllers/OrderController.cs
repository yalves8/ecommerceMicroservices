using Microsoft.AspNetCore.Mvc;
using OrderService.OrderService.Contracts;
using OrderService.OrderService.Contracts.Events;
using OrderService.OrderService.Domain.Entities;
using OrderService.OrderService.Messaging;
using OrderService.OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IMessageBusClient _bus;

        public OrderController(ILogger<OrderController> logger, IMessageBusClient bus)
        {
            _logger = logger;
            _bus = bus;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Order
            {
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // 2. Criar evento de integração
            var evt = new SalesConfirmedEvent
            {
                OrderId = order.Id,
                Items = order.Items.Select(i => new SalesConfirmedItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
            // Change this line:
            // OrderId = order.Id,

            // 3. Publicar no RabbitMQ
            await _bus.PublishOrderConfirmed(evt);

            _logger.LogInformation("Order {OrderId} created and event published", order.Id);

            return Ok(order);
        }
    }
}
