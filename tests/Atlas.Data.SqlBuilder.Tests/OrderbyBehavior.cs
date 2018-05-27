using Atlas.Data.SqlBuilder.Tests.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Atlas.Data.SqlBuilder.Tests
{
    [TestClass]
    public class OrderbyBehavior
    {
        private SqlBuilder<OrderAttr> _builderAttr;

        [TestInitialize]
        public void InitializeTest()
        {
            _builderAttr = SqlBuilder<OrderAttr>.Initialize();
        }

        [TestMethod]
        public void OrderBy_Asc()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[Orders] AS o\r\nORDER BY o.[TotalAmount] ASC\r\n";
            var sql = _builderAttr.OrderBy(o => o.TotalAmount).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void OrderBy_Desc()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[Orders] AS o\r\nORDER BY o.[TotalAmount] DESC\r\n";
            var sql = _builderAttr.OrderByDesc(o => o.TotalAmount).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }

        [TestMethod]
        public void OrderBy_MultipleOrderBy()
        {
            const string expectedResult = "SELECT\t*\r\nFROM [ec].[Orders] AS o\r\nORDER BY o.[OrderDate] ASC, o.[TotalAmount] DESC\r\n";
            var sql = _builderAttr.OrderBy(o => o.OrderDate).OrderByDesc(j => j.TotalAmount).ToSql();
            Assert.AreEqual(expectedResult, sql);
        }
    }
}
