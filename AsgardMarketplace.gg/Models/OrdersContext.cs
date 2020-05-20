using Microsoft.EntityFrameworkCore;

namespace AsgardMarketplace.gg.Models
{
    public class OrdersContext : DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options)
            : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
    }
}
