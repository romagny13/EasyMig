using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class DropDatabaseCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {

            var command = new DropDatabaseCommand("db1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("DROP DATABASE IF EXISTS [db1];\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new DropDatabaseCommand("db1");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("DROP DATABASE IF EXISTS `db1`;\r", result);
        }
    }
}
