using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib.Query.MySqlClient;
using EasyMigLib.Query.SqlClient;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class CreateStoredProcedureCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new CreateStoredProcedureCommand("p1").SetBody("select * from users");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("CREATE PROCEDURE [dbo].[p1] \rAS\rBEGIN\rselect * from users;\rEND\rGO\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithParameters_WithSql()
        {
            var command = new CreateStoredProcedureCommand("p2")
                .AddParameter("@id", ColumnType.Int())
                .AddParameter("@age", ColumnType.Int(), DatabaseParameterDirection.OUT)
                .SetBody("select @age=age from users where id=@id");
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("CREATE PROCEDURE [dbo].[p2] @id INT,@age INT OUT\rAS\rBEGIN\rselect @age=age from users where id=@id;\rEND\rGO\r", result);
        }

        [TestMethod]
        public void TestGetQuery()
        {
            var command = new CreateStoredProcedureCommand("p1").SetBody("select * from users");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("CREATE PROCEDURE `p1`()\rBEGIN\rselect * from users;\rEND", result);
        }

        [TestMethod]
        public void TestGetQuery_WithParameters()
        {
            var command = new CreateStoredProcedureCommand("p2")
                .AddParameter("p_id", ColumnType.Int())
                .AddParameter("p_age", ColumnType.Int(), DatabaseParameterDirection.OUT)
                .SetBody("select age into p_age from users where id=p_id");
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("CREATE PROCEDURE `p2`(p_id INT,OUT p_age INT)\rBEGIN\rselect age into p_age from users where id=p_id;\rEND", result);
        }

    }
}
