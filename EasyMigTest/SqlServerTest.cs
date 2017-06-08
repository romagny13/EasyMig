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
        private string connectionString = @"Server=localhost\SQLEXPRESS;Database=sql_db1;Trusted_Connection=True;";
        private string shortConnectionString = @"Server=localhost\SQLEXPRESS;Trusted_Connection=True;";

        public void BeforeEach()
        {
            EasyMig.ClearMigrations();
            EasyMig.ClearSeeders();
        }

        [TestMethod]
        public async Task TestDatabaseCreation()
        {
            this.BeforeEach();

            await TestCreateTable();

            CreateAndGetTableInfos();

            await TestAlterTable_AddColumn();

            await TestAlterTable_ModifyColumn();

            await TestAlterTable_AddPrimaryConstraint();

            await TestAlterTable_AddForeignConstraint();

            await TestAlterTable_DropColumn();

            TestSeed();

            TestSeed_WithSeedShortCut();

            // stored procedure

            TestDropStoredProcedure();

            TestCreateStoredProcedure();
        }

        [TestMethod]
        public async Task TestCreateDropDatabaseCreation()
        {
            this.BeforeEach();

            await TestDropDatabase();
            await TestCreateDatabase();
        }

        //[TestMethod]
        //public void TestProcedure()
        //{
        //    this.BeforeEach();
        //    TestDropStoredProcedure();
        //    TestCreateStoredProcedure();
        //}

        public async Task TestDropDatabase()
        {
            var dbName = "sql_db2";

            EasyMig.DropDatabase(dbName);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, shortConnectionString);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.Information.SqlServer.DatabaseExists(dbName, shortConnectionString));
        }


        public async Task TestCreateDatabase()
        {
            var dbName = "sql_db2";

            EasyMig.CreateDatabase(dbName);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, shortConnectionString);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.Information.SqlServer.DatabaseExists(dbName, shortConnectionString));
        }

        public async Task TestCreateTable()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddColumn(columnName, ColumnType.Int(true))
                .AddColumn("title");

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.Information.SqlServer.TableExists(dbName, tableName, connectionString));

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public void CreateAndGetTableInfos()
        {
            var dbName = "sql_db1";
            var tableName = "sql_users";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddPrimaryKey(columnName)
                .AddColumn("username")
                .AddColumn("age", ColumnType.Int(), true);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);
            Assert.IsNotNull(table);
        }

        public async Task TestAlterTable_AddColumn()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "user_id";

            Assert.IsFalse(EasyMig.Information.SqlServer.ColumnExists(dbName, tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).AddColumn(columnName, ColumnType.Int(true));

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.Information.SqlServer.ColumnExists(dbName, tableName, columnName, connectionString));

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public async Task TestAlterTable_ModifyColumn()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.Information.SqlServer.ColumnExists(dbName, tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).ModifyColumn(columnName, ColumnType.VarChar(), true);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.AreEqual("YES", table.GetColumn(columnName)["IS_NULLABLE"]);
        }

        public async Task TestAlterTable_AddPrimaryConstraint()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "id";

            EasyMig.AlterTable(tableName).AddPrimaryKeyConstraint(columnName);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.AreEqual(true, table.IsPrimaryKey(columnName));
        }

        public async Task TestAlterTable_AddForeignConstraint()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "id";

            var fk = "user_id";
            var tableReferenced = "sql_users";

            EasyMig.AlterTable(tableName).AddForeignKeyConstraint(fk, tableReferenced, columnName);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.AreEqual(true, table.IsForeignKey(fk));
        }

        public async Task TestAlterTable_DropColumn()
        {
            var dbName = "sql_db1";
            var tableName = "sql_posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.Information.SqlServer.ColumnExists(dbName, tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).DropColumn(columnName);

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetMigrationQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.Information.SqlServer.ColumnExists(dbName, tableName, columnName, connectionString));

            var table = EasyMig.Information.SqlServer.GetTable(dbName, tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsFalse(table.HasColumn(columnName));
        }

        public void TestSeed()
        {
            var tableName = "sql_users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user1").Set("age", 20));

            EasyMig.ToSqlServer.DoSeedFromMemory(connectionString);

            //var query = EasyMig.ToSqlServer.GetSeedQuery();

            //EasyMig.ToSqlServer.ExecuteQuery(query, connectionString);

            var tableRows = EasyMig.Information.SqlServer.GetTableRows(tableName, connectionString);
            Assert.AreEqual(1, tableRows.Count);
            Assert.AreEqual(1, ((int)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));
        }

        public void TestSeed_WithSeedShortCut()
        {
            var tableName = "sql_users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user2").Set("age", 30));

            EasyMig.ToSqlServer.DoSeedFromMemory(connectionString);

            var tableRows = EasyMig.Information.SqlServer.GetTableRows(tableName, connectionString);
            Assert.AreEqual(2, tableRows.Count);

            Assert.AreEqual(1, ((int)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));

            Assert.AreEqual(2, ((int)tableRows[1]["id"]));
            Assert.AreEqual("user2", (string)tableRows[1]["username"]);
            Assert.AreEqual(30, ((int)tableRows[1]["age"]));
        }

        public void TestDropStoredProcedure()
        {
            var dbName = "sql_db1";


            EasyMig.DropStoredProcedure("p1_test");

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            Assert.IsFalse(EasyMig.Information.SqlServer.ProcedureExists(dbName, "p1_test", connectionString));
        }

        public void TestCreateStoredProcedure()
        {
            var dbName = "sql_db1";

            EasyMig.CreateStoredProcedure("p1_test")
               .AddParameter("@id", ColumnType.Int())
               .AddParameter("@age", ColumnType.Int(), DatabaseParameterDirection.OUT)
               .SetBody("select @age=age from users where id=@id");

            EasyMig.ToSqlServer.DoMigrationsFromMemory(connectionString);

            Assert.IsTrue(EasyMig.Information.SqlServer.ProcedureExists(dbName, "p1_test", connectionString));
        }
    }
}
