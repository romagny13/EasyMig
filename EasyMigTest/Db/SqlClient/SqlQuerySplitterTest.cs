using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Db.SqlClient;

namespace EasyMigTest
{
    [TestClass]
    public class SqlQuerySplitterTest
    {
        [TestMethod]
        public void TestSplit_WithNoGO_ReturnsOneResult()
        {
            var service = new SqlQuerySplitter();
            var result = service.SplitQuery(@"instruction1;
                                                           instruction2;
                                                           instruction3;
                                                           instruction4;");


            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public void TestSplit_WithGO()
        {
            var service = new SqlQuerySplitter();
            var result = service.SplitQuery(@"closure1;
                                              endclosure1
                                              GO
                                              closure2;
                                              endclosure2
                                              GO
                                              instruction1;
                                              instruction2;");


            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("closure1;\r\n                                              endclosure1", result[0]);
            Assert.AreEqual("closure2;\r\n                                              endclosure2", result[1]);
            Assert.AreEqual("instruction1;\r\n                                              instruction2;", result[2]);
        }

    }
}
