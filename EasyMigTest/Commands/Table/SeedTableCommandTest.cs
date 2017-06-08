using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using System.Collections.Generic;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigLibTest.Commands.Table
{
    [TestClass]
    public class SeedTableCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var values = new Dictionary<string, object> { { "colum1", 1 },{ "column2", "value 2"} };

            var command = new SeedRwoCommand("table1",values );
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([colum1],[column2]) VALUES (1,'value 2');\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var values = new Dictionary<string, object> { { "colum1", 1 }, { "column2", "value 2" } };

            var command = new SeedRwoCommand("table1", values);
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("INSERT INTO `table1` (`colum1`,`column2`) VALUES (1,'value 2');\r", result);
        }
    }
}
