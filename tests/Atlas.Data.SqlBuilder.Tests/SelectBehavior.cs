using Atlas.Data.SqlBuilder.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atlas.Data.SqlBuilder.Tests
{
    [TestClass]
    public class SelectBehavior
    {
        private SqlBuilder<Order> _builder;
        private SqlBuilder<OrderAttr> _builderAttr;

        [TestInitialize]
        public void InitializeTest()
        {
            _builder = SqlBuilder<Order>.Initialize();
            _builderAttr = SqlBuilder<OrderAttr>.Initialize();
        }

        [TestMethod]
        public void Select_All_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [dbo].[Order] AS dboOrder\r\n";
            var sql = _builder.ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_AllOnlyFromMainTable_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\tdboOrder.*\r\nFROM [dbo].[Order] AS dboOrder\r\n";
            var sql = _builder.Select(f => f).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_OnlyOneColumnFromMainTable_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\tdboOrder.[Id]\r\nFROM [dbo].[Order] AS dboOrder\r\n";
            var sql = _builder.Select(f => f.Id).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_TwoColumnsFromMainTable_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\tdboOrder.[Id],\r\n\tdboOrder.[OrderDate]\r\nFROM [dbo].[Order] AS dboOrder\r\n";
            var sql = _builder.Select(f => f.Id, f => f.OrderDate).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_AllOnlyFromJoinedTable_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\tCustomer_dboCustomer.*\r\nFROM [dbo].[Order] AS dboOrder\r\n\tINNER JOIN [dbo].[Customer] AS Customer_dboCustomer\r\n\t\tON dboOrder.[CustomerId] = Customer_dboCustomer.[Id]\r\n";
            var sql = _builder.Select(f => f.Customer).Join(f => f.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_OnlyOneColumnFromJoinedTable_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\tCustomer_dboCustomer.[CustomerId]\r\nFROM [dbo].[Order] AS dboOrder\r\n\tINNER JOIN [dbo].[Customer] AS Customer_dboCustomer\r\n\t\tON dboOrder.[CustomerId] = Customer_dboCustomer.[Id]\r\n";
            var sql = _builder.Select(f => f.Customer.FirstName).Join(f => f.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_All_WithTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[Orders] AS o\r\n";
            var sql = _builderAttr.ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_AllOnlyFromMainTable_WithTableAttributes()
        {
            const string expectedResult = "SELECT\to.*\r\nFROM [ec].[Orders] AS o\r\n";
            var sql = _builderAttr.Select(f => f).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_OnlyOneColumnFromMainTable_WithTableAttributes()
        {
            const string expectedResult = "SELECT\to.[Id]\r\nFROM [ec].[Orders] AS o\r\n";
            var sql = _builderAttr.Select(f => f.Id).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_TwoColumnsFromMainTable_WithTableAttributes()
        {
            const string expectedResult = "SELECT\to.[Id],\r\n\to.[OrderDate]\r\nFROM [ec].[Orders] AS o\r\n";
            var sql = _builderAttr.Select(f => f.Id, f => f.OrderDate).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_AllOnlyFromJoinedTable_WithTableAttributes()
        {
            const string expectedResult = "SELECT\toc.*\r\nFROM [ec].[Orders] AS o\r\n\tLEFT JOIN [ec].[Customers] AS oc\r\n\t\tON o.[CustomerId] = oc.[Id]\r\n";
            var sql = _builderAttr.Select(f => f.Customer).Join(f => f.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Select_OnlyOneColumnFromJoinedTable_WithTableAttributes()
        {
            const string expectedResult = "SELECT\toc.[CustomerId]\r\nFROM [ec].[Orders] AS o\r\n\tLEFT JOIN [ec].[Customers] AS oc\r\n\t\tON o.[CustomerId] = oc.[Id]\r\n";
            var sql = _builderAttr.Select(f => f.Customer.FirstName).Join(f => f.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }
    }
}
