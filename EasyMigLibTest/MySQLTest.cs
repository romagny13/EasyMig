using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib;
using System.Threading.Tasks;
using EasyMigLib.Commands;

namespace EasyMigLibTest
{
    [TestClass]
    public class MySQLTest
    {
        private string connectionString = "server=localhost;user id=root"; 
        private string completeConnectionString = "server=localhost;database=db1;uid=root";

        private string providerName = "MySql.Data.MySqlClient";


        public void BeforeEach()
        {
            EasyMig.ClearMigrations();
            EasyMig.ClearSeeders();
        }

        [TestMethod]
        public async Task TestDatabaseCreation()
        {
            await TestDropDatabase();

            await TestCreateDatabase();

            await TestCreateTable();

            CreateAndGetTableInfos();

            await TestAlterTable_AddColumn();

            await TestAlterTable_ModifyColumn();

            await TestAlterTable_AddPrimaryConstraint();

            await TestAlterTable_AddForeignConstraint();

            await TestAlterTable_DropColumn();

            TestSeed();

            TestSeed_WithSeedShortCut();
        }

        public async Task TestDropDatabase()
        {
            this.BeforeEach();

            EasyMig.DropDatabase("db1");
            var query = EasyMig.GetMigrationQuery(providerName);

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.DatabaseInformation.DatabaseExists("db1", connectionString, providerName));
        }


        public async Task TestCreateDatabase()
        {
            this.BeforeEach();

            EasyMig.CreateDatabase("db1");
            var query = EasyMig.GetMigrationQuery(providerName);

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.DatabaseInformation.DatabaseExists("db1", connectionString, providerName));
        }

        public async Task TestCreateTable()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddColumn(columnName, ColumnType.Int(true))
                .AddColumn("title");

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.DatabaseInformation.TableExists(dbName, tableName, connectionString, providerName));

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public void CreateAndGetTableInfos()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "users";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddPrimaryKey(columnName)
                .AddColumn("username")
                .AddColumn("age", ColumnType.Int(),true);

            EasyMig.DoMigrationsFromMemory(connectionString, providerName);

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);
            Assert.IsNotNull(table);
        }

        public async Task TestAlterTable_AddColumn()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "user_id";

            Assert.IsFalse(EasyMig.DatabaseInformation.ColumnExists(dbName, tableName, columnName, connectionString, providerName));

            EasyMig.AlterTable(tableName).AddColumn(columnName, ColumnType.Int(true));

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.DatabaseInformation.ColumnExists(dbName, tableName, columnName, connectionString, providerName));

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public async Task TestAlterTable_ModifyColumn()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.DatabaseInformation.ColumnExists(dbName, tableName, columnName, connectionString, providerName));

            EasyMig.AlterTable(tableName).ModifyColumn(columnName, ColumnType.String(), true, "default value");

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.AreEqual("YES", table.GetColumn(columnName)["IS_NULLABLE"]);
            Assert.AreEqual("default value", table.GetColumn(columnName)["COLUMN_DEFAULT"]);
        }

        public async Task TestAlterTable_AddPrimaryConstraint()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "id";

            EasyMig.AlterTable(tableName).AddPrimaryKeyConstraint(tableName, new string[] { columnName });

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.AreEqual(true, table.IsPrimaryKey(columnName));
        }

        public async Task TestAlterTable_AddForeignConstraint()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "id";

            var fk = "user_id";
            var tableReferenced = "users";

            EasyMig.AlterTable(tableName).AddForeignKeyConstraint(fk,tableReferenced,columnName);

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.AreEqual(true, table.IsForeignKey(fk));
        }

        public async Task TestAlterTable_DropColumn()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.DatabaseInformation.ColumnExists(dbName, tableName, columnName, connectionString, providerName));

            EasyMig.AlterTable(tableName).DropColumn(columnName);

            var query = EasyMig.GetMigrationQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.DatabaseInformation.ColumnExists(dbName, tableName, columnName, connectionString, providerName));

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.IsNotNull(table);
            Assert.IsFalse(table.HasColumn(columnName));
        }

        public void TestSeed()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username","user1").Set("age",20));

            var query = EasyMig.GetSeedQuery(providerName);

            query = "USE `" + dbName + "`;\r" + query;

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            var tableRows = EasyMig.DatabaseInformation.GetTableRows(tableName, connectionString, providerName);
            Assert.AreEqual(1, tableRows.Count);
            Assert.AreEqual((uint)1,((uint) tableRows[0]["id"]));
            Assert.AreEqual("user1", (string) tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));
        }

        public void TestSeed_WithSeedShortCut()
        {
            this.BeforeEach();

            var tableName = "users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user2").Set("age", 30));

            EasyMig.DoSeedFromMemory(completeConnectionString, providerName);

            var tableRows = EasyMig.DatabaseInformation.GetTableRows(tableName, completeConnectionString, providerName);
            Assert.AreEqual(2, tableRows.Count);

            Assert.AreEqual((uint)1, ((uint)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));

            Assert.AreEqual((uint)2, ((uint)tableRows[1]["id"]));
            Assert.AreEqual("user2", (string)tableRows[1]["username"]);
            Assert.AreEqual(30, ((int)tableRows[1]["age"]));
        }

    }
}
