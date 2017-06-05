using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class AddForeignKeyConstraintCommandTest
    {
        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new AddForeignKeyConstraintCommand("posts", new ForeignKeyColumn("user_id", ColumnType.Int(), "users","id"));
            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\r", result);
        }


        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new AddForeignKeyConstraintCommand("posts", new ForeignKeyColumn("user_id", ColumnType.Int(), "users", "id"));
            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("ALTER TABLE `posts` ADD FOREIGN KEY (`user_id`) REFERENCES `users`(`id`);\r", result);
        }

    }
}
