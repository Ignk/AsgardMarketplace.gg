using AsgardMarketplace.gg.Models;
using System.Threading.Tasks;

namespace AsgardMarketplace.gg.Services
{
    public interface INotificationService
    {
        Task SendPaidNotification(Order order);
        Task SendDeliveredNotification(Order order);
    }
}
