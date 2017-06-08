using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class ModifyColumnCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new ModifyColumnCommand("table1", new MigrationColumn("column1", ColumnType.VarChar(), true));
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ALTER COLUMN [column1] NVARCHAR(255) NULL;\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new ModifyColumnCommand("table1", new MigrationColumn("column1", ColumnType.VarChar(), true));
            var result = command.GetQuery(new MySqlQueryService());

            Assert.AreEqual("ALTER TABLE `table1` MODIFY COLUMN `column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NULL;\r", result);
        }

    }
}
