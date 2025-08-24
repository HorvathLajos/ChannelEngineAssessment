using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChannelEngine.Business.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<OrderLine> Lines { get; set; } = [];
    }

    public class OrderLine
    {
        public string MerchantProductNo { get; set; } = string.Empty;
        public string Gtin { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public OrderLineStockLocation? StockLocation { get; set; }
    }

    public class OrderLineStockLocation
    {
        public int Id { get; set; }
    }

    public class OrderResponse
    {
        public List<Order> Content { get; set; } = [];
    }
}
