using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib;
using EasyMigLib.Commands;
using System.Threading.Tasks;
namespace EasyMigLibTest
{
    [TestClass]
    public class SqlServerTest
    {
        // Server=localhost\SQLEXPRESS;Database=master;Trusted_Connection=True;

        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=db1;Trusted_Connection=True;";
        private string shortConnectionString = @"Server=localhost\SQLEXPRESS;Trusted_Connection=True;";

        private string providerName = "System.Data.SqlClient";


        public void BeforeEach()
        {
            EasyMig.ClearMigrations();
            EasyMig.ClearSeeders();
        }

        [TestMethod]
        public async Task TestDatabaseCreation()
        {

            await TestCreateTable();

            CreateAndGetTableInfos();

            await TestAlterTable_AddColumn();

            await TestAlterTable_ModifyColumn();

            await TestAlterTable_AddPrimaryConstraint();

            await TestAlterTable_AddForeignConstraint();

            await TestAlterTable_DropColumn();

            TestSeed();

            TestSeed_WithSeedShortCut();

            await TestCreateDatabase();

            await TestDropDatabase();

        }

        public async Task TestDropDatabase()
        {
            this.BeforeEach();

            var dbName = "db2";

            EasyMig.DropDatabase(dbName);
            var query = EasyMig.GetMigrationQuery(providerName);


            EasyMig.ExecuteQuery(query, shortConnectionString, providerName);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.DatabaseInformation.DatabaseExists(dbName, connectionString, providerName));
        }


        public async Task TestCreateDatabase()
        {
            this.BeforeEach();

            var dbName = "db2";

            EasyMig.CreateDatabase(dbName);
            var query = EasyMig.GetMigrationQuery(providerName);

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.DatabaseInformation.DatabaseExists(dbName, connectionString, providerName));
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
                .AddColumn("age", ColumnType.Int(), true);

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

            EasyMig.AlterTable(tableName).ModifyColumn(columnName, ColumnType.String(), true);

            var query = EasyMig.GetMigrationQuery(providerName);

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            await Task.Delay(500);

            var table = EasyMig.DatabaseInformation.GetTable(dbName, tableName, connectionString, providerName);

            Assert.AreEqual("YES", table.GetColumn(columnName)["IS_NULLABLE"]);
        }

        public async Task TestAlterTable_AddPrimaryConstraint()
        {
            this.BeforeEach();

            var dbName = "db1";
            var tableName = "posts";
            var columnName = "id";

            EasyMig.AlterTable(tableName).AddPrimaryKeyConstraint(tableName, new string[] { columnName });

            var query = EasyMig.GetMigrationQuery(providerName);

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

            EasyMig.AlterTable(tableName).AddForeignKeyConstraint(fk, tableReferenced, columnName);

            var query = EasyMig.GetMigrationQuery(providerName);

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

            var tableName = "users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user1").Set("age", 20));

            var query = EasyMig.GetSeedQuery(providerName);

            EasyMig.ExecuteQuery(query, connectionString, providerName);

            var tableRows = EasyMig.DatabaseInformation.GetTableRows(tableName, connectionString, providerName);
            Assert.AreEqual(1, tableRows.Count);
            Assert.AreEqual(1, ((int)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));
        }

        public void TestSeed_WithSeedShortCut()
        {
            this.BeforeEach();

            var tableName = "users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user2").Set("age", 30));

            EasyMig.DoSeedFromMemory(connectionString, providerName);

            var tableRows = EasyMig.DatabaseInformation.GetTableRows(tableName, connectionString, providerName);
            Assert.AreEqual(2, tableRows.Count);

            Assert.AreEqual(1, ((int)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));

            Assert.AreEqual(2, ((int)tableRows[1]["id"]));
            Assert.AreEqual("user2", (string)tableRows[1]["username"]);
            Assert.AreEqual(30, ((int)tableRows[1]["age"]));
        }
    }
}
