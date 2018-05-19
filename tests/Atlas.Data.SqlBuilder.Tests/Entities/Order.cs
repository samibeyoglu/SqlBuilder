using System;
using System.Collections.Generic;

namespace Atlas.Data.SqlBuilder.Tests.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid CustomerId { get; set; }

        public Customer Customer { get; set; }

        public List<OrderDetail> Details { get; set; }
    }
}
