using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

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
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\r", result);
        }
    }
}
