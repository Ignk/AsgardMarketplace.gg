using System;

namespace AsgardMarketplace.gg.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Item { get; set; }
        public string Customer { get; set; }
        public DateTime Ordered { get; set; }
        public bool Paid { get; set; }
        public bool Delivered { get; set; }
    }
}
