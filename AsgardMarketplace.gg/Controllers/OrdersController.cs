using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsgardMarketplace.gg.Models;
using System;
using AsgardMarketplace.gg.Services;

namespace AsgardMarketplace.gg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase , IOrdersController
    {
        private readonly OrdersContext _context;
        private readonly INotificationService _notificationService;

        public OrdersController(OrdersContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/customer?customer=Tom
        [Route("customer")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(string customer)
        {
            var orders = await _context.Orders.Where(o => o.Customer == customer).ToListAsync();

            if (!orders.Any())
            {
                return NotFound();
            }

            return orders;
        }

        // GET: api/Orders/delivered
        [Route("delivered")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetDeliveredOrders()
        {
            return await _context.Orders.Where(o => o.Delivered).ToListAsync();
        }

        // GET: api/Orders/paid
        [Route("paid")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetPaidOrders()
        {
            return await _context.Orders.Where(o => o.Paid).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // POST: api/Orders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(NewOrderRequest request)
        {
            var order = new Order
            {
                Ordered = DateTime.Now,
                Item = request.Item,
                Customer = request.Customer
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        // POST: api/Orders/pay
        [Route("pay")]
        [HttpPost()]
        public async Task<IActionResult> PayOrder(PayOrderRequest request)
        {
            var order = await _context.Orders.FindAsync(request.Id);
            if (order == null || order.Customer != request.Customer)
            {
                return BadRequest();
            }

            order.Paid = true;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _notificationService.SendPaidNotification(order);

            return Ok();
        }

        // POST: api/Orders/deliver
        [Route("deliver")]
        [HttpPost()]
        public async Task<IActionResult> DeliverOrder(DeliverOrderRequest request)
        {
            var order = await _context.Orders.FindAsync(request.Id);
            if (order == null || order.Customer != request.Customer)
            {
                return BadRequest();
            }

            order.Delivered = true;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _notificationService.SendDeliveredNotification(order);
            
            return Ok();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

    }
}
