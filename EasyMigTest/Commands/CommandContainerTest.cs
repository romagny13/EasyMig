using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using System.Collections.Generic;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigTest.Commands
{
    [TestClass]
    public class CommandContainerTest
    {
        // check

        [TestMethod]
        public void TestCreateDatabase_WithSameName_Fail()
        {
            bool failed = false;
            var name = "db1";

            var container = new CommandContainer();
            container.CreateDatabase(name);

            try
            {
                container.CreateDatabase(name);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateAndUseDatabase_WithSameName_Fail()
        {
            bool failed = false;
            var name = "db1";

            var container = new CommandContainer();
            container.CreateAndUseDatabase(name);

            try
            {
                container.CreateAndUseDatabase(name);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestDropDatabase_WithSameName_Fail()
        {
            bool failed = false;
            var name = "db1";

            var container = new CommandContainer();
            container.DropDatabase(name);

            try
            {
                container.DropDatabase(name);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestDropDatabase_WithNotSameName_Success()
        {
            bool failed = false;
            var name = "db1";

            var container = new CommandContainer();
            container.DropDatabase(name);

            try
            {
                container.DropDatabase("db2");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestCreateTable_WithSameName_Fail()
        {
            bool failed = false;
            var name = "table1";

            var container = new CommandContainer();
            container.CreateTable(name);

            try
            {
                container.CreateTable(name);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateTable_WithNotSameName_Success()
        {
            bool failed = false;
            var name = "table1";

            var container = new CommandContainer();
            container.CreateTable(name);

            try
            {
                container.CreateTable("table2");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestDropTable_WithSameName_Fail()
        {
            bool failed = false;
            var name = "table1";

            var container = new CommandContainer();
            container.DropTable(name);

            try
            {
                container.DropTable(name);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestDropTable_WithNotSameName_Success()
        {
            bool failed = false;
            var name = "table1";

            var container = new CommandContainer();
            container.DropTable(name);

            try
            {
                container.DropTable("table2");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        // create database

        [TestMethod]
        public void TestCreateDatabaseCommand()
        {
            var name = "db1";

            var container = new CommandContainer();

            container.CreateDatabase(name);

            Assert.IsTrue(container.HasCreateDatabaseCommand(name));

            var result = container.GetCreateDatabaseCommand(name);

            Assert.AreEqual(typeof(CreateDatabaseCommand), result.GetType());
            Assert.AreEqual(name, result.DatabaseName);
        }

        [TestMethod]
        public void TestCreateAndUseDatabaseCommand()
        {
            var name = "db1";

            var container = new CommandContainer();

            container.CreateAndUseDatabase(name);

            Assert.IsTrue(container.HasCreateAndUseDatabaseCommand(name));

            var result = container.GetCreateAndUseDatabaseCommand(name);

            Assert.AreEqual(typeof(CreateAndUseDatabaseCommand), result.GetType());
            Assert.AreEqual(name, result.DatabaseName);
        }

        // drop database

        [TestMethod]
        public void TestDropDatabaseCommand()
        {
            var name = "db1";

            var container = new CommandContainer();

            container.DropDatabase(name);

            Assert.IsTrue(container.HasDropDatabaseCommand(name));

            var result = container.GetDropDatabaseCommand(name);

            Assert.AreEqual(typeof(DropDatabaseCommand), result.GetType());
            Assert.AreEqual(name, result.DatabaseName);
        }

        // create table

        [TestMethod]
        public void TestCreateTableCommand()
        {
            var name = "table1";

            var container = new CommandContainer();

            container.CreateTable(name);

            Assert.IsTrue(container.HasCreateTableCommand(name));

            var result = container.GetCreateTableCommand(name);

            Assert.AreEqual(typeof(CreateTableCommand), result.GetType());
            Assert.AreEqual(name, result.TableName);
        }



        // alter table

        [TestMethod]
        public void TestAlterTableCommand()
        {
            var name = "table1";

            var container = new CommandContainer();

            container.AlterTable(name)
                .AddColumn("column1");

            Assert.IsTrue(container.HasAlterTableCommand(name));

            var result = container.GetAlterTableCommand(name);

            Assert.AreEqual(name, result.TableName);
            Assert.AreEqual(true, result.HasAddColumnCommands);
        }


        [TestMethod]
        public void TestAlterTableCommand_ReturnsOldTable()
        {
            var name = "table1";

            var container = new CommandContainer();

            container.AlterTable(name)
                   .AddColumn("column1");

            Assert.IsTrue(container.HasAlterTableCommand(name));

            container.AlterTable(name)
                 .AddColumn("column2");

            Assert.IsTrue(container.GetAlterTableCommand(name).HasAddColumnCommand("column1"));
            Assert.IsTrue(container.GetAlterTableCommand(name).HasAddColumnCommand("column2"));
        }

        // drop table

        [TestMethod]
        public void TestDropTableCommand()
        {
            var name = "table1";

            var container = new CommandContainer();

            container.DropTable(name);

            Assert.IsTrue(container.HasDropTableCommand(name));

            var result = container.GetDropTableCommand(name);

            Assert.AreEqual(name, result.TableName);
        }

        // seed

        [TestMethod]
        public void TestSeed()
        {
            var tableName = "table1";

            var container = new CommandContainer();

            container.SeedTable(tableName)
                .Insert(new Dictionary<string, object> { { "id", 1 }, { "column1", "value 1" } })
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            Assert.IsTrue(container.HasSeedTable(tableName));

            var seedTable = container.GetSeedTable(tableName);

            Assert.AreEqual(2, seedTable.RowCount);

            var row = seedTable.GetRow(0);
            Assert.AreEqual(1, row["id"]);
            Assert.AreEqual("value 1", row["column1"]);

            var row2 = seedTable.GetRow(1);
            Assert.AreEqual(2, row2["id"]);
            Assert.AreEqual("value 2", row2["column1"]);
        }

        [TestMethod]
        public void TestSeed_ReturnsOldTable()
        {
            var tableName = "table1";

            var container = new CommandContainer();

            container.SeedTable(tableName)
                .Insert(new Dictionary<string, object> { { "id", 1 }, { "column1", "value 1" } })
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            Assert.IsTrue(container.HasSeedTable(tableName));

            var seedTable = container.GetSeedTable(tableName);

            Assert.AreEqual(2, seedTable.RowCount);

            container.SeedTable(tableName)
                .Insert(SeedData.New.Set("id", 3).Set("column1", "value 3"));

            Assert.AreEqual(3, container.GetSeedTable(tableName).RowCount);

            var row = seedTable.GetRow(2);
            Assert.AreEqual(3, row["id"]);
            Assert.AreEqual("value 3", row["column1"]);
        }

        [TestMethod]
        public void TestClearSeeders()
        {
            var container = new CommandContainer();

            container.SeedTable("table1").Insert(SeedData.New.Set("column1", "value1"));


            Assert.IsTrue(container.HasSeedCommands);

            container.ClearSeeders();

            Assert.IsFalse(container.HasSeedCommands);
        }

        [TestMethod]
        public void TestClearMigrations()
        {
            var container = new CommandContainer();

            container.DropDatabase("db1");
            container.CreateDatabase("db1");
            container.CreateAndUseDatabase("db1");

            container.CreateTable("table1");
            container.AlterTable("table1").AddColumn("column1");
            container.DropTable("table1");

            container.DropStoredProcedure("p1");
            container.CreateStoredProcedure("p1");

            Assert.IsTrue(container.HasCreateDatabaseCommands);
            Assert.IsTrue(container.HasCreateAndUseDatabaseCommands);
            Assert.IsTrue(container.HasDropDatabaseCommands);
            Assert.IsTrue(container.HasCreateTableCommands);
            Assert.IsTrue(container.HasAlterTableCommands);
            Assert.IsTrue(container.HasDropTableCommands);
            Assert.IsTrue(container.HasCreateStoredProcedureCommands);
            Assert.IsTrue(container.HasDropStoredProcedureCommands);

            container.ClearMigrations();

            Assert.IsFalse(container.HasCreateDatabaseCommands);
            Assert.IsFalse(container.HasCreateAndUseDatabaseCommands);
            Assert.IsFalse(container.HasDropDatabaseCommands);
            Assert.IsFalse(container.HasCreateTableCommands);
            Assert.IsFalse(container.HasAlterTableCommands);
            Assert.IsFalse(container.HasDropTableCommands);
            Assert.IsFalse(container.HasCreateStoredProcedureCommands);
            Assert.IsFalse(container.HasDropStoredProcedureCommands);
        }


        // sql queries

        [TestMethod]
        public void TestGetDropDatabasesQuery()
        {
            var container = new CommandContainer();

            container.DropDatabase("db1");
            container.DropDatabase("db2");

            var result =  container.GetDropDatabasesQuery(new SqlQueryService());

            Assert.AreEqual("DROP DATABASE IF EXISTS [db1]\rGO\r\rDROP DATABASE IF EXISTS [db2]\rGO\r", result);
        }

        [TestMethod]
        public void TestGetCreateDatabasesQuery()
        {
            var container = new CommandContainer();

            container.CreateDatabase("db1");
            container.CreateDatabase("db2");

            var result = container.GetCreateDatabasesQuery(new SqlQueryService());

            Assert.AreEqual("CREATE DATABASE [db1]\rGO\r\rCREATE DATABASE [db2]\rGO\r", result);
        }

        [TestMethod]
        public void TestGetCreateAndUseDatabasesQuery()
        {
            var container = new CommandContainer();

            container.CreateAndUseDatabase("db1");

            var result = container.GetCreateAndUseDatabasesQuery(new SqlQueryService());

            Assert.AreEqual("CREATE DATABASE [db1]\rGO\rUSE [db1]\rGO\r", result);
        }

        [TestMethod]
        public void TestGeCreateTablesQuery()
        {
            var container = new CommandContainer();

            container.CreateTable("table1").AddPrimaryKey("id").AddColumn("column1");

            container.CreateTable("table2").AddForeignKey("table1_id","table1","id");

            var result = container.GetCreateTablesQuery(new SqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table2]\rGO\r\rDROP TABLE IF EXISTS [dbo].[table1]\rGO\r\rCREATE TABLE [dbo].[table1] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL\r)\rGO\r\rCREATE TABLE [dbo].[table2] (\r\t[table1_id] INT NOT NULL\r)\rGO\r\rALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([id])\rGO\r\rALTER TABLE [dbo].[table2] ADD FOREIGN KEY ([table1_id]) REFERENCES [dbo].[table1]([id])\rGO\r", result);
        }

        [TestMethod]
        public void TestGetDropTablesQuery()
        {
            var container = new CommandContainer();

            container.DropTable("table1");

            container.DropTable("table2");

            var result = container.GetDropTablesQuery(new SqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1]\rGO\rDROP TABLE IF EXISTS [dbo].[table2]\rGO\r", result);
        }

        [TestMethod]
        public void TestGetAlterTablesQuery()
        {
            var container = new CommandContainer();

            container.AlterTable("table1").AddColumn("column1").AddColumn("column2");

            container.AlterTable("table2").AddColumn("column1").AddColumn("column2");

            var result = container.GetAlterTablesQuery(new SqlQueryService());

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD [column1] NVARCHAR(255) NOT NULL\rGO\r\rALTER TABLE [dbo].[table1] ADD [column2] NVARCHAR(255) NOT NULL\rGO\r\rALTER TABLE [dbo].[table2] ADD [column1] NVARCHAR(255) NOT NULL\rGO\r\rALTER TABLE [dbo].[table2] ADD [column2] NVARCHAR(255) NOT NULL\rGO\r", result);
        }

        [TestMethod]
        public void TestDropStoredProceduresQuery()
        {
            var container = new CommandContainer();

            container.DropStoredProcedure("p1");

            container.DropStoredProcedure("p2");

            var result = container.GetDropStoredProceduresQuery(new SqlQueryService());

            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p1]\rGO\r\rDROP PROCEDURE IF EXISTS [dbo].[p2]\rGO\r", result);
        }

        [TestMethod]
        public void TestSeedQuery()
        {
            var container = new CommandContainer();

            container.SeedTable("table1").Insert(SeedData.New.Set("column1", "value1")).Insert(SeedData.New.Set("column1", "value2"));

            container.SeedTable("table2").Insert(SeedData.New.Set("column1", "value1")).Insert(SeedData.New.Set("column1", "value2"));

           var result =  container.GetSeedQuery(new SqlQueryService());

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([column1]) VALUES ('value1')\rGO\rINSERT INTO [dbo].[table1] ([column1]) VALUES ('value2')\rGO\r\rINSERT INTO [dbo].[table2] ([column1]) VALUES ('value1')\rGO\rINSERT INTO [dbo].[table2] ([column1]) VALUES ('value2')\rGO\r", result);
        }


        // mysql queries

        [TestMethod]
        public void TestGetDropDatabasesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.DropDatabase("db1");
            container.DropDatabase("db2");

            var result = container.GetDropDatabasesQuery(new MySqlQueryService());

            Assert.AreEqual("DROP DATABASE IF EXISTS `db1`;\r\rDROP DATABASE IF EXISTS `db2`;\r", result);
        }

        [TestMethod]
        public void TestGetCreateDatabasesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.CreateDatabase("db1");
            container.CreateDatabase("db2");

            var result = container.GetCreateDatabasesQuery(new MySqlQueryService());

            Assert.AreEqual("CREATE DATABASE `db1`;\r\rCREATE DATABASE `db2`;\r", result);
        }

        [TestMethod]
        public void TestGetCreateAndUseDatabasesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.CreateAndUseDatabase("db1");

            var result = container.GetCreateAndUseDatabasesQuery(new MySqlQueryService());

            Assert.AreEqual("CREATE DATABASE `db1`;\rUSE `db1`;\r", result);
        }

        [TestMethod]
        public void TestGeCreateTablesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.CreateTable("table1").AddPrimaryKey("id").AddColumn("column1");

            container.CreateTable("table2").AddForeignKey("table1_id", "table1", "id");

            var result = container.GetCreateTablesQuery(new MySqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS `table2`;\r\rDROP TABLE IF EXISTS `table1`;\r\rCREATE TABLE `table1` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r\rCREATE TABLE `table2` (\r\t`table1_id` INT UNSIGNED NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r\rALTER TABLE `table1` ADD PRIMARY KEY (`id`);\rALTER TABLE `table1` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r\rCREATE INDEX `table1_id_index` ON `table2` (`table1_id`);\rALTER TABLE `table2` ADD FOREIGN KEY (`table1_id`) REFERENCES `table1`(`id`);\r", result);
        }

        [TestMethod]
        public void TestGetDropTablesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.DropTable("table1");

            container.DropTable("table2");

            var result = container.GetDropTablesQuery(new MySqlQueryService());

            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\rDROP TABLE IF EXISTS `table2`;\r", result);
        }

        [TestMethod]
        public void TestGetAlterTablesQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.AlterTable("table1").AddColumn("column1").AddColumn("column2");

            container.AlterTable("table2").AddColumn("column1").AddColumn("column2");

            var result = container.GetAlterTablesQuery(new MySqlQueryService());

            Assert.AreEqual("ALTER TABLE `table1` ADD `column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r\rALTER TABLE `table1` ADD `column2` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r\rALTER TABLE `table2` ADD `column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r\rALTER TABLE `table2` ADD `column2` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r", result);
        }

        [TestMethod]
        public void TestDropStoredProceduresQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.DropStoredProcedure("p1");

            container.DropStoredProcedure("p2");

            var result = container.GetDropStoredProceduresQuery(new MySqlQueryService());

            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p1`;\r\rDROP PROCEDURE IF EXISTS `p2`;\r", result);
        }

        [TestMethod]
        public void TestSeedQuery_WithMySql()
        {
            var container = new CommandContainer();

            container.SeedTable("table1").Insert(SeedData.New.Set("column1", "value1")).Insert(SeedData.New.Set("column1", "value2"));

            container.SeedTable("table2").Insert(SeedData.New.Set("column1", "value1")).Insert(SeedData.New.Set("column1", "value2"));

            var result = container.GetSeedQuery(new MySqlQueryService());

            Assert.AreEqual("INSERT INTO `table1` (`column1`) VALUES ('value1');\rINSERT INTO `table1` (`column1`) VALUES ('value2');\r\rINSERT INTO `table2` (`column1`) VALUES ('value1');\rINSERT INTO `table2` (`column1`) VALUES ('value2');\r", result);
        }
    }
}
