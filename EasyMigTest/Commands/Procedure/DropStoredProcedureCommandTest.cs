using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Query.MySqlClient;
using EasyMigLib.Commands;
using EasyMigLib.Query.SqlClient;

namespace EasyMigTest.Commands.MySqlClient.Procedure
{
    [TestClass]
    public class DropStoredProcedureCommandTest
    {

        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new DropStoredProcedureCommand("p1");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p1]\rGO\r", result);
        }

        [TestMethod]
        public void TestGetQuery()
        {
            var command = new DropStoredProcedureCommand("p1");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p1`;\r", result);
        }
    }
}
