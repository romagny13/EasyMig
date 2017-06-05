using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Services;

namespace EasyMigLibTest.Services
{
    [TestClass]
    public class QueryServiceFactoriesTest
    {
        [TestMethod]
        public void TestGetSqlQueryService()
        {
            var service = QueryServiceFactories.GetService("System.Data.SqlClient");
            Assert.AreEqual(typeof(SqlQueryService), service.GetType());
        }

        [TestMethod]
        public void TestGetMySQLQueryService()
        {
            var service = QueryServiceFactories.GetService("MySql.Data.MySqlClient");
            Assert.AreEqual(typeof(MySQLQueryService), service.GetType());
            Assert.AreEqual("InnoDB", ((MySQLQueryService)service).Engine);
        }

        [TestMethod]
        public void TestGetMySQLQueryService_WithEngine()
        {
            var service = QueryServiceFactories.GetService("MySql.Data.MySqlClient", "MyISAM");
            Assert.AreEqual(typeof(MySQLQueryService), service.GetType());
            Assert.AreEqual("MyISAM", ((MySQLQueryService)service).Engine);
        }


        [TestMethod]
        public void TestGetService_WithNoSupported_Fail()
        {
            bool failed = false;
            try
            {
                var service = QueryServiceFactories.GetService("not supported");
            }
            catch (Exception)
            {
                failed = true;
            }
            Assert.IsTrue(failed);
        }
    }
}
