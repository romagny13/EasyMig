using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib.Query.MySqlClient;
using EasyMigLib.Query.SqlClient;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class DropTableCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new DropTableCommand("table1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new DropTableCommand("table1");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\r", result);
        }
    }
}
