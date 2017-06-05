using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class AddPrimaryKeyConstraintCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var keys = new string[] { "column1" };
            var command = new AddPrimaryKeyConstraintCommand("table1", keys);
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([column1]);\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithColumnsAndSql()
        {
            var keys = new string[] { "column1", "column2" };
            var command = new AddPrimaryKeyConstraintCommand("table1", keys);
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([column1],[column2]);\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var keys = new string[] { "column1" };
            var command = new AddPrimaryKeyConstraintCommand("table1", keys);
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("ALTER TABLE `table1` ADD PRIMARY KEY (`column1`);\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithColumnsAndMySQL()
        {
            var keys = new string[] { "column1", "column2" };
            var command = new AddPrimaryKeyConstraintCommand("table1", keys);
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("ALTER TABLE `table1` ADD PRIMARY KEY (`column1`,`column2`);\r", result);
        }
    }
}
