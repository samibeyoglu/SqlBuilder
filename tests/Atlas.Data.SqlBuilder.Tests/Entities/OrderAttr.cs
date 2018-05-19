using System;
using System.Collections.Generic;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder.Tests.Entities
{
    [Table(Schema = "ec", Name = "Orders", Alias = "o")]
    public class OrderAttr
    {
        public Guid Id { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid CustomerId { get; set; }

        [Join(JoinType = JoinType.Left, ColumnName = "CustomerId", ForeignSchema = "ec", ForeignTableName = "Customers", ForeignColumnName = "Id", Alias = "oc")]
        public CustomerAttr Customer { get; set; }

        [Join(JoinType = JoinType.Left, ColumnName = "Id", ForeignSchema = "ec", ForeignTableName = "OrderDetails", ForeignColumnName = "OrderId", Alias = "od")]
        public List<OrderDetailAttr> Details { get; set; }
    }
}
