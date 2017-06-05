using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class DropColumnCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new DropColumnCommand("table1", "column1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[table1] DROP COLUMN [column1];\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new DropColumnCommand("table1", "column1");
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("ALTER TABLE `table1` DROP COLUMN `column1`;\r", result);
        }

    }
}
