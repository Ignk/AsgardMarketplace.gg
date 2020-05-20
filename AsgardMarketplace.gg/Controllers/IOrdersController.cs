using AsgardMarketplace.gg.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsgardMarketplace.gg.Controllers
{
    public interface IOrdersController
    {
        Task<ActionResult<IEnumerable<Order>>> GetOrders();
        Task<ActionResult<IEnumerable<Order>>> GetUserOrders(string customer);
        Task<ActionResult<IEnumerable<Order>>> GetDeliveredOrders();
        Task<ActionResult<IEnumerable<Order>>> GetPaidOrders();
        Task<ActionResult<Order>> GetOrder(int id);
        Task<ActionResult<Order>> PostOrder(NewOrderRequest request);
        Task<ActionResult<Order>> DeleteOrder(int id);
        Task<IActionResult> PayOrder(PayOrderRequest request);
        Task<IActionResult> DeliverOrder(DeliverOrderRequest request);
    }
}
