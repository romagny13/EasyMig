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
        public void TestColumnTypes()
        {
            EasyMig.CreateTable("column_test")
                .AddColumn("my_char", ColumnType.Char(50))
                .AddColumn("my_varchar", ColumnType.VarChar(50))
                .AddColumn("my_text", ColumnType.Text())
                .AddColumn("my_longtext", ColumnType.LongText())
                .AddColumn("my_tiny", ColumnType.TinyInt())
                .AddColumn("my_small", ColumnType.SmallInt())
                .AddColumn("my_int", ColumnType.Int())
                .AddColumn("my_big", ColumnType.BigInt())
                .AddColumn("my_bit", ColumnType.Bit())
                .AddColumn("my_float", ColumnType.Float(2))
                .AddColumn("my_datetime", ColumnType.DateTime())
                .AddColumn("my_date", ColumnType.Date())
                .AddColumn("my_time", ColumnType.Time())
                .AddColumn("my_timestamp", ColumnType.Timestamp())
                .AddColumn("my_blob", ColumnType.Blob());

            EasyMig.DoMigrationsFromMemory(this.completeConnectionString, this.providerName);

           var result =  EasyMig.DatabaseInformation.GetTable("db1","column_test", this.completeConnectionString, this.providerName);

            Assert.AreEqual("char", (string)result.Columns["my_char"]["DATA_TYPE"]);
            Assert.AreEqual((UInt64)50, (UInt64)result.Columns["my_char"]["CHARACTER_MAXIMUM_LENGTH"]);
            Assert.AreEqual("varchar", (string)result.Columns["my_varchar"]["DATA_TYPE"]);
            Assert.AreEqual((UInt64)50, (UInt64)result.Columns["my_varchar"]["CHARACTER_MAXIMUM_LENGTH"]);
            Assert.AreEqual("text", (string)result.Columns["my_text"]["DATA_TYPE"]);
            Assert.AreEqual("longtext", (string)result.Columns["my_longtext"]["DATA_TYPE"]);
            Assert.AreEqual("tinyint",  (string)result.Columns["my_tiny"]["DATA_TYPE"]);
            Assert.AreEqual("smallint", (string)result.Columns["my_small"]["DATA_TYPE"]);
            Assert.AreEqual("int", (string)result.Columns["my_int"]["DATA_TYPE"]);
            Assert.AreEqual("bigint", (string)result.Columns["my_big"]["DATA_TYPE"]);
            Assert.AreEqual("bit", (string)result.Columns["my_bit"]["DATA_TYPE"]);
            Assert.AreEqual("float", (string)result.Columns["my_float"]["DATA_TYPE"]);
            Assert.AreEqual("datetime", (string)result.Columns["my_datetime"]["DATA_TYPE"]);
            Assert.AreEqual("date", (string)result.Columns["my_date"]["DATA_TYPE"]);
            Assert.AreEqual("time", (string)result.Columns["my_time"]["DATA_TYPE"]);
            Assert.AreEqual("timestamp", (string)result.Columns["my_timestamp"]["DATA_TYPE"]);
            Assert.AreEqual("blob", (string)result.Columns["my_blob"]["DATA_TYPE"]);
        }

        [TestMethod]
        public void TestDefaultValues()
        {
            EasyMig.CreateTable("column_default_values")
                .AddColumn("my_char", ColumnType.Char(50), true, "default char")
                .AddColumn("my_varchar", ColumnType.VarChar(50), true, "default varchar")
                .AddColumn("my_text", ColumnType.Text())
                .AddColumn("my_longtext", ColumnType.LongText())
                .AddColumn("my_tiny", ColumnType.TinyInt(), true, 10) // int or string
                .AddColumn("my_small", ColumnType.SmallInt(), true, 20)
                .AddColumn("my_int", ColumnType.Int(), true, 30)
                .AddColumn("my_big", ColumnType.BigInt(), true, 40)
                .AddColumn("my_bit", ColumnType.Bit(), true, 1) // int
                .AddColumn("my_float", ColumnType.Float(2), true, "10.99")
                .AddColumn("my_datetime", ColumnType.DateTime(), true, "CURRENT_TIMESTAMP") // CURRENT_TIMESTAMP || NULL
                .AddColumn("my_date", ColumnType.Date()) 
                .AddColumn("my_time", ColumnType.Time()) 
                .AddColumn("my_timestamp", ColumnType.Timestamp(), true, "CURRENT_TIMESTAMP")
                .AddColumn("my_blob", ColumnType.Blob());

            EasyMig.DoMigrationsFromMemory(this.completeConnectionString, this.providerName);

            var result = EasyMig.DatabaseInformation.GetTable("db1", "column_default_values", this.completeConnectionString, this.providerName);

            Assert.AreEqual("default char", (string)result.Columns["my_char"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("default varchar", (string)result.Columns["my_varchar"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("10", (string)result.Columns["my_tiny"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("20", (string)result.Columns["my_small"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("30", (string)result.Columns["my_int"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("40", (string)result.Columns["my_big"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("b'1'", (string)result.Columns["my_bit"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("10.99", (string)result.Columns["my_float"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("CURRENT_TIMESTAMP", (string)result.Columns["my_datetime"]["COLUMN_DEFAULT"]);
            Assert.AreEqual("CURRENT_TIMESTAMP", (string)result.Columns["my_timestamp"]["COLUMN_DEFAULT"]);
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

            EasyMig.AlterTable(tableName).ModifyColumn(columnName, ColumnType.VarChar(), true, "default value");

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

            EasyMig.AlterTable(tableName).AddPrimaryKeyConstraint(columnName);

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


