using System;
using Atlas.Data.SqlBuilder.Enums;
using Atlas.Data.SqlBuilder.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atlas.Data.SqlBuilder.Tests
{
    [TestClass]
    public class WhereBehavior
    {
        private SqlBuilder<OrderDetail> _builder;
        private SqlBuilder<OrderDetailAttr> _builderAttr;

        [TestInitialize]
        public void InitializeTest()
        {
            _builder = SqlBuilder<OrderDetail>.Initialize();
            _builderAttr = SqlBuilder<OrderDetailAttr>.Initialize();
        }

        [TestMethod]
        public void Where_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [dbo].[OrderDetail] AS dboOrderDetail\r\nWHERE dboOrderDetail.[ProductName] = 'Value'\r\n";
            var sql = _builder.Where(w => w.ProductName, ComparisonOperator.Equal, "Value").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_WithTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] = 'Value'\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.Equal, "Value").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_NotEqualOperator()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] <> 'Value'\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.NotEqual, "Value").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_GraterThanOperator()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] > 0\r\n";
            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.GraterThan, 0M).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_GraterOrEqualOperator()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] >= 0\r\n";
            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.GraterOrEqual, 0M).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }


        [TestMethod]
        public void Where_LessThanOperator()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] < 100\r\n";
            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.LessThan, 100M).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_LessOrEqualOperator()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] <= 100\r\n";
            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.LessOrEqual, 100M).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_IsNull()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] IS NULL\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.Equal, default(string)).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_IsNotNull()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] IS NOT NULL\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.NotEqual, default(string)).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_Like()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] LIKE '%Value%'\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.Like, "%Value%").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_NotLike()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] NOT LIKE '%Value%'\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.NotLike, "%Value%").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_In()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] IN ('Value1','Value2')\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.In, "Value1", "Value2").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_NotIn()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductName] NOT IN ('Value1','Value2')\r\n";
            var sql = _builderAttr.Where(w => w.ProductName, ComparisonOperator.NotIn, "Value1", "Value2").ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_Between()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] BETWEEN 50 AND 100\r\n";
            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.Between, 50, 100).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Guid()
        {
            var id = new Guid("2dcbb1b8-2576-4f65-a8b3-f8467caeb289");
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Id] = '2dcbb1b8-2576-4f65-a8b3-f8467caeb289'\r\n";

            var sql = _builderAttr.Where(w => w.Id, ComparisonOperator.Equal, id).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Float()
        {
            var weight = 3.14F;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Weight] = 3.14\r\n";

            var sql = _builderAttr.Where(w => w.Weight, ComparisonOperator.Equal, weight).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Decimal()
        {
            float price = 100;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Price] = 100\r\n";

            var sql = _builderAttr.Where(w => w.Price, ComparisonOperator.Equal, price).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Boolean()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[HasGiftPackage] = TRUE\r\n";

            var sql = _builderAttr.Where(w => w.HasGiftPackage, ComparisonOperator.Equal, true).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_DateTimeOffset()
        {
            var timeToDeliver = new DateTimeOffset(2018, 05, 26, 10, 59, 12, 58, new TimeSpan(3, 0, 0));
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[TimeToDeliver] = '2018-05-26 10:59:12.0580 +03:00'\r\n";

            var sql = _builderAttr.Where(w => w.TimeToDeliver, ComparisonOperator.Equal, timeToDeliver).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_DateTime()
        {
            var dateOfProduction = new DateTime(2018,05,26);
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[DateOfProduction] = '2018-05-26 00:00:00.0000'\r\n";

            var sql = _builderAttr.Where(w => w.DateOfProduction, ComparisonOperator.Equal, dateOfProduction).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Short()
        {
            short quantity = 3;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Quantity] = 3\r\n";

            var sql = _builderAttr.Where(w => w.Quantity, ComparisonOperator.Equal, quantity).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Double()
        {
            double sampleDoubleProperty = 3000.123456;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[SampleDoubleProperty] = 3000.123456\r\n";

            var sql = _builderAttr.Where(w => w.SampleDoubleProperty, ComparisonOperator.Equal, sampleDoubleProperty).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Long()
        {
            long barcode = 86123456789;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Barcode] = 86123456789\r\n";

            var sql = _builderAttr.Where(w => w.Barcode, ComparisonOperator.Equal, barcode).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Int()
        {
            int sku = 123456;
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[Sku] = 123456\r\n";

            var sql = _builderAttr.Where(w => w.Sku, ComparisonOperator.Equal, sku).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_Enum()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ProductType] = 2\r\n";

            var sql = _builderAttr.Where(w => w.ProductType, ComparisonOperator.Equal, (byte)ProductType.Artwork).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Where_DataType_TimeSpan()
        {
            var shelfLife=new TimeSpan(364,59,59);
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[OrderDetails] AS o\r\nWHERE o.[ShelfLife] = 13139990000000\r\n";

            var sql = _builderAttr.Where(w => w.ShelfLife, ComparisonOperator.Equal, shelfLife).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }
    }
}
