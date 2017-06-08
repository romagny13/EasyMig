using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using EasyMigLib;
using EasyMigLib.Commands;

namespace EasyMigTest
{
    [TestClass]
    public class SqlServerFileTest
    {
 
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\experimental\EasyMigLib\EasyMigTest\dbTest.mdf;Integrated Security=True;Connect Timeout=30";

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

            TestDropStoredProcedure();

            TestCreateStoredProcedure();
        }

        //[TestMethod]
        //public void TestProcedure()
        //{
        //    this.BeforeEach();

        //    TestDropStoredProcedure();
        //    TestCreateStoredProcedure();
        //}

        public async Task TestCreateTable()
        {
            this.BeforeEach();

            var tableName = "sqlfile_posts";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddColumn(columnName, ColumnType.Int(true))
                .AddColumn("title");

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.Information.SqlServerAttachedDbFile.TableExists(tableName, connectionString));

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public void CreateAndGetTableInfos()
        {
            this.BeforeEach();

            var tableName = "sqlfile_users";
            var columnName = "id";

            EasyMig.CreateTable(tableName)
                .AddPrimaryKey(columnName)
                .AddColumn("username")
                .AddColumn("age", ColumnType.Int(), true);

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);
            Assert.IsNotNull(table);
        }

        public async Task TestAlterTable_AddColumn()
        {
            this.BeforeEach();

            var tableName = "sqlfile_posts";
            var columnName = "user_id";

            Assert.IsFalse(EasyMig.Information.SqlServerAttachedDbFile.ColumnExists(tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).AddColumn(columnName, ColumnType.Int(true));

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsTrue(EasyMig.Information.SqlServerAttachedDbFile.ColumnExists(tableName, columnName, connectionString));

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsTrue(table.HasColumn(columnName));
        }

        public async Task TestAlterTable_ModifyColumn()
        {

            var tableName = "sqlfile_posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.Information.SqlServerAttachedDbFile.ColumnExists(tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).ModifyColumn(columnName, ColumnType.VarChar(), true);

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.AreEqual("YES", table.GetColumn(columnName)["IS_NULLABLE"]);
        }

        public async Task TestAlterTable_AddPrimaryConstraint()
        {
            var tableName = "sqlfile_posts";
            var columnName = "id";

            EasyMig.AlterTable(tableName).AddPrimaryKeyConstraint(columnName);

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.AreEqual(true, table.IsPrimaryKey(columnName));
        }

        public async Task TestAlterTable_AddForeignConstraint()
        {
            var tableName = "sqlfile_posts";
            var columnName = "id";

            var fk = "user_id";
            var tableReferenced = "sqlfile_users";

            EasyMig.AlterTable(tableName).AddForeignKeyConstraint(fk, tableReferenced, columnName);

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.AreEqual(true, table.IsForeignKey(fk));
        }

        public async Task TestAlterTable_DropColumn()
        {
            var tableName = "sqlfile_posts";
            var columnName = "title";

            Assert.IsTrue(EasyMig.Information.SqlServerAttachedDbFile.ColumnExists(tableName, columnName, connectionString));

            EasyMig.AlterTable(tableName).DropColumn(columnName);

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetMigrationQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            await Task.Delay(500);

            Assert.IsFalse(EasyMig.Information.SqlServerAttachedDbFile.ColumnExists(tableName, columnName, connectionString));

            var table = EasyMig.Information.SqlServerAttachedDbFile.GetTable(tableName, connectionString);

            Assert.IsNotNull(table);
            Assert.IsFalse(table.HasColumn(columnName));
        }

        public void TestSeed()
        {
            var tableName = "sqlfile_users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user1").Set("age", 20));

            EasyMig.ToSqlServerAttachedDbFile.DoSeedFromMemory(connectionString);

            //var query = EasyMig.ToSqlServerAttachedDbFile.GetSeedQuery();

            //EasyMig.ToSqlServerAttachedDbFile.ExecuteQuery(query, connectionString);

            var tableRows = EasyMig.Information.SqlServerAttachedDbFile.GetTableRows(tableName, connectionString);
            Assert.AreEqual(1, tableRows.Count);
            Assert.AreEqual(1, ((int)tableRows[0]["id"]));
            Assert.AreEqual("user1", (string)tableRows[0]["username"]);
            Assert.AreEqual(20, ((int)tableRows[0]["age"]));
        }

        public void TestSeed_WithSeedShortCut()
        {
            var tableName = "sqlfile_users";

            EasyMig.SeedTable(tableName)
                .Insert(SeedData.New.Set("username", "user2").Set("age", 30));

            EasyMig.ToSqlServerAttachedDbFile.DoSeedFromMemory(connectionString);

            var tableRows = EasyMig.Information.SqlServerAttachedDbFile.GetTableRows(tableName, connectionString);
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
            EasyMig.DropStoredProcedure("p1_test");

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            Assert.IsFalse(EasyMig.Information.SqlServerAttachedDbFile.ProcedureExists("p1_test", connectionString));
        }

        public void TestCreateStoredProcedure()
        {
            EasyMig.CreateStoredProcedure("p1_test")
               .AddParameter("@id", ColumnType.Int())
               .AddParameter("@age", ColumnType.Int(), DatabaseParameterDirection.OUT)
               .SetBody("select @age=age from users where id=@id");

            EasyMig.ToSqlServerAttachedDbFile.DoMigrationsFromMemory(connectionString);

            Assert.IsTrue(EasyMig.Information.SqlServerAttachedDbFile.ProcedureExists("p1_test", connectionString));
        }
    }
}
