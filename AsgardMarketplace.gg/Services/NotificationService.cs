using AsgardMarketplace.gg.Models;
using System.Threading.Tasks;

namespace AsgardMarketplace.gg.Services
{
    public class NotificationService : INotificationService
    {
        public async Task SendPaidNotification(Order order)
        {
            //There should be integration with some kind of notification service (SMTP, SMS, Push Notification, etc.) 

        }

        public async Task SendDeliveredNotification(Order order)
        {
            //There should be integration with some kind of notification service (SMTP, SMS, Push Notification, etc.) 
        }
    }
}
