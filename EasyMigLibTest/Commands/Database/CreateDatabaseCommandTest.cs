using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class CreateDatabaseCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new CreateDatabaseCommand("db1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("CREATE DATABASE [db1];\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new CreateDatabaseCommand("db1");
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("CREATE DATABASE `db1`;\r", result);
        }
    }
}
