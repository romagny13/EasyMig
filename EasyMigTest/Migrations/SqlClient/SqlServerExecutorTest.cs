using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Schema;
using EasyMigLib.Migrations.SqlClient;
using EasyMigLib.Db.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMigLib.MigrationReflection;
using EasyMigTest.Migrations.MySql;
using EasyMigLib.Query.SqlClient;

namespace EasyMigTest.Migrations.SqlClient
{
    [TestClass]
    public class SqlServerExecutorTest
    {
        // queries

        // database

        [TestMethod]
        public void TestGetDropDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropDatabase("db1");
            schema.DropDatabase("db2");

            var service = new SqlServerExecutor(schema);

            var result = service.GetDropDatabasesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP DATABASE IF EXISTS [db1];\r", result[0]);
            Assert.AreEqual("DROP DATABASE IF EXISTS [db2];\r", result[1]);
        }

        [TestMethod]
        public void TestGetCreateDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateDatabase("db1");
            schema.CreateDatabase("db2");

            var service = new SqlServerExecutor(schema);

            var result = service.GetCreateDatabasesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("CREATE DATABASE [db1];\r", result[0]);
            Assert.AreEqual("CREATE DATABASE [db2];\r", result[1]);
        }

        [TestMethod]
        public void TestGetCreateAndUseDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateAndUseDatabase("db1");
            schema.CreateAndUseDatabase("db2");

            var service = new SqlServerExecutor(schema);

            var result = service.GetCreateAndUseDatabasesQueries();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("CREATE DATABASE [db1];\r", result[0]);
            Assert.AreEqual("USE [db1];\r", result[1]);
            Assert.AreEqual("CREATE DATABASE [db2];\r", result[2]);
            Assert.AreEqual("USE [db2];\r", result[3]);
        }

        // tables

        [TestMethod]
        public void TestGetCreateTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateTable("table1")
                .AddPrimaryKey("id")
                .AddColumn("column1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            schema.CreateTable("table2").AddPrimaryKey("id").AddColumn("column1").AddForeignKey("table1_id", "table1", "id");

            var service = new SqlServerExecutor(schema);

            var result = service.GetCreateTablesQueries();

            Assert.AreEqual(11, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table2];\r", result[0]);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result[1]);
            Assert.AreEqual("CREATE TABLE [dbo].[table1] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL\r);\r", result[2]);
            Assert.AreEqual("CREATE TABLE [dbo].[table2] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[table1_id] INT NOT NULL\r);\r", result[3]);
            Assert.AreEqual("SET IDENTITY_INSERT [dbo].[table1] ON;\r", result[4]);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1');\r", result[5]);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2');\r", result[6]);
            Assert.AreEqual("SET IDENTITY_INSERT [dbo].[table1] OFF;\r", result[7]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([id]);\r", result[8]);
            Assert.AreEqual("ALTER TABLE [dbo].[table2] ADD PRIMARY KEY ([id]);\r", result[9]);
            Assert.AreEqual("ALTER TABLE [dbo].[table2] ADD FOREIGN KEY ([table1_id]) REFERENCES [dbo].[table1]([id]);\r", result[10]);
        }

        [TestMethod]
        public void TestGetAlterTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.AlterTable("table1")
              .AddColumn("column1")
              .AddColumn("column2")
              .ModifyColumn("column3", ColumnType.Int())
              .ModifyColumn("column4", ColumnType.Int())
              .DropColumn("column5")
              .DropColumn("column6")
              .AddPrimaryKeyConstraint("id")
              .AddForeignKeyConstraint("table2_id", "table2", "id")
              .AddForeignKeyConstraint("table3_id", "table3", "id");

            var service = new SqlServerExecutor(schema);

            var result = service.GetAlterTablesQueries();

            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD [column1] NVARCHAR(255) NOT NULL;\r", result[0]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD [column2] NVARCHAR(255) NOT NULL;\r", result[1]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ALTER COLUMN [column3] INT NOT NULL;\r", result[2]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ALTER COLUMN [column4] INT NOT NULL;\r", result[3]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] DROP COLUMN [column5];\r", result[4]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] DROP COLUMN [column6];\r", result[5]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([id]);\r", result[6]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD FOREIGN KEY ([table2_id]) REFERENCES [dbo].[table2]([id]);\r", result[7]);
            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD FOREIGN KEY ([table3_id]) REFERENCES [dbo].[table3]([id]);\r", result[8]);
        }

        [TestMethod]
        public void TestGetDropTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropTable("table1");
            schema.DropTable("table2");

            var service = new SqlServerExecutor(schema);

            var result = service.GetDropTablesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result[0]);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table2];\r", result[1]);
        }

        // stored procedures

        [TestMethod]
        public void TestGetCreateStoredProceduresQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateStoredProcedure("p1").AddInParameter("p_id", ColumnType.Int()).SetBody("select * from users where id=p_id");
            schema.CreateStoredProcedure("p2").SetBody("select * from users");

            var service = new SqlServerExecutor(schema);

            var result = service.GetCreateStoredProceduresQueries();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p1];\r", result[0]);
            Assert.AreEqual("CREATE PROCEDURE [dbo].[p1] p_id INT\rAS\rBEGIN\rselect * from users where id=p_id;\rEND;\r", result[1]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p2];\r", result[2]);
            Assert.AreEqual("CREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND;\r", result[3]);
        }

        [TestMethod]
        public void TestGetDropStoredProceduresQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropStoredProcedure("p1");
            schema.DropStoredProcedure("p2");

            var service = new SqlServerExecutor(schema);

            var result = service.GetDropStoredProceduresQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p1];\r", result[0]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p2];\r", result[1]);
        }

        // seeders

        [TestMethod]
        public void TestGetSeedQueries()
        {
            var schema = new DatabaseSchema();
            schema.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            var service = new SqlServerExecutor(schema);

            var result = service.GetSeedQueriesWithSemicolonDelimiter();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1');\r", result[0]);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2');\r", result[1]);
        }

        // script

        [TestMethod]
        public void TestGetMigrationQuery()
        {

            var schema = new DatabaseSchema();
            schema.CreateTable("table1")
                .AddPrimaryKey("id")
                .AddColumn("column1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            schema.CreateTable("table2").AddPrimaryKey("id")
                .AddColumn("column1")
                .AddForeignKey("table1_id", "table1", "id")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"));

            schema.CreateStoredProcedure("p1").AddInParameter("p_id", ColumnType.Int()).SetBody("select * from users where id=p_id");
            schema.CreateStoredProcedure("p2").SetBody("select * from users");

            var service = new SqlServerExecutor(schema);

            var result = service.GetMigrationQueryWithGODelimiter();

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table2]\rGO\r\rDROP TABLE IF EXISTS [dbo].[table1]\rGO\r\rCREATE TABLE [dbo].[table1] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL\r)\rGO\r\rCREATE TABLE [dbo].[table2] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[table1_id] INT NOT NULL\r)\rGO\r\rSET IDENTITY_INSERT [dbo].[table1] ON\rGO\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1')\rGO\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2')\rGO\rSET IDENTITY_INSERT [dbo].[table1] OFF\rGO\r\rSET IDENTITY_INSERT [dbo].[table2] ON\rGO\rINSERT INTO [dbo].[table2] ([id],[column1]) VALUES (1,'value1')\rGO\rSET IDENTITY_INSERT [dbo].[table2] OFF\rGO\r\rALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([id])\rGO\r\rALTER TABLE [dbo].[table2] ADD PRIMARY KEY ([id])\rGO\r\rALTER TABLE [dbo].[table2] ADD FOREIGN KEY ([table1_id]) REFERENCES [dbo].[table1]([id])\rGO\r\rDROP PROCEDURE IF EXISTS [dbo].[p1]\rGO\r\rCREATE PROCEDURE [dbo].[p1] p_id INT\rAS\rBEGIN\rselect * from users where id=p_id;\rEND\rGO\r\rDROP PROCEDURE IF EXISTS [dbo].[p2]\rGO\r\rCREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND\rGO\r", result);
        }

        [TestMethod]
        public void TestGetSeedQuery()
        {
            var schema = new DatabaseSchema();
            schema.SeedTable("table1")
                .Insert(SeedData.New.Set("column1", "value1"))
                .Insert(SeedData.New.Set("column1", "value2"));

            schema.SeedTable("table2")
              .Insert(SeedData.New.Set("column1", "value1"))
              .Insert(SeedData.New.Set("column1", "value2"));

            var service = new SqlServerExecutor(schema);

            var result = service.GetSeedQueryWithGODelimiter();

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([column1]) VALUES ('value1')\rGO\rINSERT INTO [dbo].[table1] ([column1]) VALUES ('value2')\rGO\r\rINSERT INTO [dbo].[table2] ([column1]) VALUES ('value1')\rGO\rINSERT INTO [dbo].[table2] ([column1]) VALUES ('value2')\rGO\r", result);
        }

        // switch delimiter

        [TestMethod]
        public void TestSwitchDelimiter()
        {
            var schema = new DatabaseSchema();
            schema.CreateTable("table1")
                .AddColumn("column1");

            schema.CreateStoredProcedure("p2").SetBody("select * from users");

            var service = new SqlServerExecutor(schema);

            // ;/r
            var result = service.GetMigrationQueriesWithSemicolonDelimiter(false);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result[0]);
            Assert.AreEqual("CREATE TABLE [dbo].[table1] (\r\t[column1] NVARCHAR(255) NOT NULL\r);\r", result[1]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p2];\r", result[2]);
            Assert.AreEqual("CREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND;\r", result[3]);

            // /rGO/r
            var result2 = service.GetMigrationQueryWithGODelimiter();

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1]\rGO\r\rCREATE TABLE [dbo].[table1] (\r\t[column1] NVARCHAR(255) NOT NULL\r)\rGO\r\rDROP PROCEDURE IF EXISTS [dbo].[p2]\rGO\r\rCREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND\rGO\r", result2);

            var result3 = service.GetMigrationQueriesWithSemicolonDelimiter(false);

            Assert.AreEqual(4, result3.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result3[0]);
            Assert.AreEqual("CREATE TABLE [dbo].[table1] (\r\t[column1] NVARCHAR(255) NOT NULL\r);\r", result3[1]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[p2];\r", result3[2]);
            Assert.AreEqual("CREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND;\r", result3[3]);

            var result4 = service.GetMigrationQueryWithGODelimiter();

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1]\rGO\r\rCREATE TABLE [dbo].[table1] (\r\t[column1] NVARCHAR(255) NOT NULL\r)\rGO\r\rDROP PROCEDURE IF EXISTS [dbo].[p2]\rGO\r\rCREATE PROCEDURE [dbo].[p2] \rAS\rBEGIN\rselect * from users;\rEND\rGO\r", result4);
        }

        [TestMethod]
        public void TestSwitchDelimiter_WithSeed()
        {
            var schema = new DatabaseSchema();
            schema.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            var service = new SqlServerExecutor(schema);

            var result = service.GetSeedQueriesWithSemicolonDelimiter();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1');\r", result[0]);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2');\r", result[1]);

            var result2 = service.GetSeedQueryWithGODelimiter();

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1')\rGO\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2')\rGO\r", result2);

            var result3 = service.GetSeedQueriesWithSemicolonDelimiter();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1');\r", result3[0]);
            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2');\r", result[1]);

            var result4 = service.GetSeedQueryWithGODelimiter();

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value1')\rGO\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value2')\rGO\r", result4);
        }

        // called


        [TestMethod]
        public void TestDbServiceIsCalledOnQueries()
        {
            var dbService = new SqlFakeDbService();
            var container = new SqlServerExecutor(new MigrationAssemblyService(),
                  new SqlQueryService(),
                  dbService,
                  new DatabaseSchema());

            container.OpenConnectionAndExecuteQueries(new List<string> { "q1", "q2" }, "");

            Assert.AreEqual(2, dbService.Results.Count);
            Assert.AreEqual("q1", dbService.Results[0]);
            Assert.AreEqual("q2", dbService.Results[1]);
        }


        [TestMethod]
        public void TestSeederIsCalled()
        {
            var container = new SqlServerExecutor(new DatabaseSchema());

            _1_Seed.IsCalled = false;

            var file = new RecognizedMigrationFile(typeof(_1_Seed), "_1_Seed", "Seed", "_1");
            container.RunSeeder(file);

            Assert.IsTrue(_1_Seed.IsCalled);
        }

        [TestMethod]
        public void TestMigrationIsCalled()
        {
            var container = new SqlServerExecutor(new DatabaseSchema());

            _1_Mig.UpIsCalled = false;
            _1_Mig.DownIsCalled = false;

            var file = new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1");
            container.RunMigration(file);

            Assert.IsTrue(_1_Mig.UpIsCalled);
            Assert.IsFalse(_1_Mig.DownIsCalled);
        }

        [TestMethod]
        public void TestMigrationIsCalled_WithDown()
        {
            var container = new SqlServerExecutor(new DatabaseSchema());

            _1_Mig.UpIsCalled = false;
            _1_Mig.DownIsCalled = false;

            var file = new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1");
            container.RunMigration(file, MigrationDirection.Down);

            Assert.IsFalse(_1_Mig.UpIsCalled);
            Assert.IsTrue(_1_Mig.DownIsCalled);
        }

        [TestMethod]
        public void TestGetLast()
        {
            var container = new SqlServerExecutor(new DatabaseSchema());

            var files = new List<RecognizedMigrationFile>{
                new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1"),
                new RecognizedMigrationFile(typeof(_2_Mig), "_2_Mig", "Mig", "_2"),
            };

            var result = container.GetLast(files);

            Assert.AreEqual("_2", result.Version);
        }
    }

    public class SqlFakeDbService : IDbService
    {
        public bool IsCalled { get; set; }
        public List<string> Results = new List<string>();

        public System.Data.Common.DbConnection Connection => throw new NotImplementedException();

        public void Close()
        {

        }

        public IDbService CreateConnection(string connectionString, string providerName)
        {
            return this;
        }

        public int Execute(string sql, List<DbServiceParameter> parameters = null)
        {
            this.Results.Add(sql);
            this.IsCalled = true;
            return 1;
        }

        public Task ExecuteAsync(string sql, List<DbServiceParameter> parameters = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql, List<DbServiceParameter> parameters = null)
        {
            throw new NotImplementedException();
        }

        public bool IsOpen()
        {
            return true;
        }

        public void Open()
        {

        }

        public Task OpenAsync()
        {
            throw new NotImplementedException();
        }

        public List<Dictionary<string, object>> ReadAll(string sql, List<DbServiceParameter> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> ReadOne(string sql, List<DbServiceParameter> parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
