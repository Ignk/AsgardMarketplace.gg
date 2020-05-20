using AsgardMarketplace.gg.Controllers;
using AsgardMarketplace.gg.Models;
using AsgardMarketplace.gg.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AsgardMarketplace.gg.Tests
{
    public class OrdersControllerTests
    {
        [Fact]
        public async void ShouldGetOrders()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetOrders()).Value.ToList();

            Assert.Equal(3, response.Count);
        }

        [Fact]
        public async void ShouldGetUserOrders()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetUserOrders("Tom")).Value.ToList();

            Assert.NotEmpty(response);
            Assert.DoesNotContain(response, o => o.Customer != "Tom");
        }

        [Fact]
        public async void ShouldGetUserOrdersReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.GetUserOrders("Maria");
            var statusCodeResult = (IStatusCodeActionResult)response.Result;

            Assert.Equal(404, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void ShouldGetDeliveredOrders()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetDeliveredOrders()).Value.ToList();

            Assert.Single(response);
        }

        [Fact]
        public async void ShouldGetPaidOrders()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetPaidOrders()).Value.ToList();

            Assert.Equal(2, response.Count);
        }

        [Fact]
        public async void ShouldGetOrder()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetOrder(1)).Value;

            Assert.Equal(1, response.Id);
        }

        [Fact]
        public async void ShouldGetOrderReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.GetOrder(99);
            var statusCodeResult = (IStatusCodeActionResult)response.Result;

            Assert.Equal(404, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void ShouldPostOrder()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);
            var order = new NewOrderRequest
            {
                Customer = "Maria",
                Item = "Ring",
            };

            await controller.PostOrder(order);
            var response = (await controller.GetUserOrders(order.Customer)).Value.FirstOrDefault();

            Assert.Equal(order.Customer, response.Customer);
            Assert.Equal(order.Item, response.Item);
            Assert.False(response.Delivered);
            Assert.False(response.Paid);
        }

        [Fact]
        public async void ShouldDeleteOrder()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = (await controller.GetOrder(1)).Value;
            Assert.NotNull(response);

            await controller.DeleteOrder(1);
            response = (await controller.GetOrder(1)).Value;
            Assert.Null(response);
        }

        [Fact]
        public async void ShouldDeleteOrderReturnsNotFound()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.DeleteOrder(99);
            var statusCodeResult = (IStatusCodeActionResult)response.Result;

            Assert.Equal(404, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void ShouldPayOrder()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.PayOrder(new PayOrderRequest { Customer = "John", Id = 2 });
            var statusCodeResult = (IStatusCodeActionResult)response;

            Assert.Equal(200, statusCodeResult.StatusCode);
            notificationMock.Verify(n => n.SendPaidNotification(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async void ShouldPayOrderReturnBadResponse()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.PayOrder(new PayOrderRequest { Customer = "John", Id = 99 });
            var statusCodeResult = (IStatusCodeActionResult)response;

            Assert.Equal(400, statusCodeResult.StatusCode);
            notificationMock.Verify(n => n.SendPaidNotification(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async void ShouldDeliverOrder()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.DeliverOrder(new DeliverOrderRequest { Customer = "Steve", Id = 1 });
            var statusCodeResult = (IStatusCodeActionResult)response;

            Assert.Equal(200, statusCodeResult.StatusCode);
            notificationMock.Verify(n => n.SendDeliveredNotification(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async void ShouldDeliverOrderReturnBadResponse()
        {
            var dbContext = await GetDatabaseContext();
            var notificationMock = new Mock<INotificationService>();
            IOrdersController controller = new OrdersController(dbContext, notificationMock.Object);

            var response = await controller.DeliverOrder(new DeliverOrderRequest { Customer = "John", Id = 99 });
            var statusCodeResult = (IStatusCodeActionResult)response;

            Assert.Equal(400, statusCodeResult.StatusCode);
            notificationMock.Verify(n => n.SendDeliveredNotification(It.IsAny<Order>()), Times.Never);
        }

        private async Task<OrdersContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<OrdersContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new OrdersContext(options);
            databaseContext.Database.EnsureCreated();
            databaseContext.Orders.AddRange(
                new Order()
                {
                    Id = 1,
                    Customer = "Steve",
                    Ordered = DateTime.Now,
                    Item = "TV",
                    Delivered = false,
                    Paid = true
                },
                new Order()
                {
                    Id = 2,
                    Customer = "John",
                    Ordered = DateTime.Now,
                    Item = "PC",
                    Delivered = false,
                    Paid = false
                },
                new Order()
                {
                    Id = 3,
                    Customer = "Tom",
                    Ordered = DateTime.Now,
                    Item = "Printer",
                    Delivered = true,
                    Paid = true
                })
                ;

            await databaseContext.SaveChangesAsync();

            return databaseContext;
        }
    }

}
