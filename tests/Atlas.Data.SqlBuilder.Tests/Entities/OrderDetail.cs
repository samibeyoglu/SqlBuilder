using System;

namespace Atlas.Data.SqlBuilder.Tests.Entities
{
    public class OrderDetail
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

        public Order Order { get; set; }
    }

    public enum ProductType : byte
    {
        Food,
        Clothing,
        Artwork
    }
}
