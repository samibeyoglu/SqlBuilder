using System;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder.Tests.Entities
{
    [Table(Schema = "ec", Name = "OrderDetails", Alias = "o")]
    public class OrderDetailAttr
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string ProductName { get; set; }

        public float Weight { get; set; }

        public decimal Price { get; set; }

        public bool HasGiftPackage { get; set; }

        public DateTimeOffset TimeToDeliver { get; set; }

        public DateTime DateOfProduction { get; set; }

        public short Quantity { get; set; }

        public double SampleDoubleProperty { get; set; }

        public long Barcode { get; set; }

        public ProductType ProductType { get; set; }

        public TimeSpan ShelfLife { get; set; }

        public int Sku { get; set; }

        [Join(JoinType = JoinType.Left, ColumnName = "OrderId", ForeignSchema = "ec", ForeignTableName = "Orders", ForeignColumnName = "Id", Alias = "o")]
        public OrderAttr Order { get; set; }
    }
}
