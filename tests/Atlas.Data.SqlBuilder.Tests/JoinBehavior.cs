using Atlas.Data.SqlBuilder.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atlas.Data.SqlBuilder.Tests
{
    [TestClass]
    public class JoinBehavior
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
        public void Join_WithoutTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [dbo].[Order] AS dboOrder\r\n\tINNER JOIN [dbo].[Customer] AS Customer_dboCustomer\r\n\t\tON dboOrder.[CustomerId] = Customer_dboCustomer.[Id]\r\n";
            var sql = _builder.Join(j => j.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void Join_WithTableAttributes()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[Orders] AS o\r\n\tLEFT JOIN [ec].[Customers] AS oc\r\n\t\tON o.[CustomerId] = oc.[Id]\r\n";
            var sql = _builderAttr.Join(j => j.Customer).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }


    }
}
