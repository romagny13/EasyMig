using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigTest.Commands.Database
{
    [TestClass]
    public class CreateAndUseDatabaseCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new CreateAndUseDatabaseCommand("db1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("CREATE DATABASE [db1]\rGO\rUSE [db1]\rGO\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new CreateAndUseDatabaseCommand("db1");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("CREATE DATABASE `db1`;\rUSE `db1`;\r", result);
        }
    }
}
