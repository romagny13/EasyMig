using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib;
using EasyMigLib.Commands;
using System.Collections.Generic;
using EasyMigLib.Services;
using System.Threading.Tasks;
using EasyMigLibTest.Services;
using System.Data.Common;

namespace EasyMigLibTest
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

            Assert.AreEqual(2,seedTable.RowCount);

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


        // get seed query

        [TestMethod]
        public void TestGetSeedQuery()
        {
            var container = new CommandContainer();
            container.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            container.SeedTable("table2")
               .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

           var query = container.GetSeedQuery("System.Data.SqlClient");

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value 1');\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value 2');\r\rINSERT INTO [dbo].[table2] ([id],[columna]) VALUES (1,'value a');\r", query);
        }


        [TestMethod]
        public void TestGetSeedOnlyFor_SeederAndDbServiceCalled()
        {
            var service = new FakeDb();

            SeederA.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(service.IsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            var recognizedFile = new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA");

            container.DoSeedOnlyFor(recognizedFile, "", "System.Data.SqlClient");

            Assert.IsTrue(SeederA.IsCalled);
            Assert.IsTrue(service.IsCalled);
        }

        [TestMethod]
        public void TestSeederIsClear()
        {
            var service = new FakeDb();

            var container = new CommandContainer(new MigrationAssemblyService(), service);
            container.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            container.SeedTable("table2")
               .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

            Assert.AreEqual(true, container.HasSeedCommands);

            var recognizedFile = new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA");
            container.DoSeedOnlyFor(recognizedFile, "", "System.Data.SqlClient");

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value 1');\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value 2');\r\rINSERT INTO [dbo].[table2] ([id],[columna]) VALUES (1,'value a');\r", service.Query);

            Assert.AreEqual(false,container.HasSeedCommands);
        }

        [TestMethod]
        public void TestGetSeedAll_SeederAndDbServiceCalled()
        {
            var service = new FakeDb();

            SeederA.IsCalled = false;
            SeederB.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(SeederB.IsCalled);
            Assert.IsFalse(service.IsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            var files = new List<RecognizedMigrationFile> {
                new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA"),
                new RecognizedMigrationFile(typeof(SeederB), "SeederB", "SeederB")
            };

            container.DoSeedAll(files, "", "System.Data.SqlClient");

            Assert.IsTrue(SeederA.IsCalled);
            Assert.IsTrue(SeederB.IsCalled);
            Assert.IsTrue(service.IsCalled);
        }

        [TestMethod]
        public void TestGetSeedAll_SeedersAreClear()
        {
            var service = new FakeDb();

            SeederA.IsCalled = false;
            SeederB.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(SeederB.IsCalled);
            Assert.IsFalse(service.IsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);
            container.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            container.SeedTable("table2")
               .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

            Assert.AreEqual(true, container.HasSeedCommands);


            var files = new List<RecognizedMigrationFile> {
                new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA"),
                new RecognizedMigrationFile(typeof(SeederB), "SeederB", "SeederB")
            };

            container.DoSeedAll(files, "", "System.Data.SqlClient");

            Assert.AreEqual(false, container.HasSeedCommands);
        }


        [TestMethod]
        public void TestGetMigrationQuery()
        {
            var container = new CommandContainer();

            container.DropDatabase("db1");
            container.CreateDatabase("db2");

            container
                .CreateTable("users")
                .AddPrimaryKey("id")
                .AddColumn("username")
                .Insert(SeedData.New.Set("id",1).Set("username","user 1"));

           container
                .CreateTable("posts")
                .AddPrimaryKey("id")
                .AddColumn("title")
                .AddForeignKey("user_id","users","id")
                .Insert(SeedData.New.Set("id", 1).Set("title", "post 1").Set("user_id",1));

            container.AlterTable("table2").AddColumn("column1");

            container.DropTable("table3");

            var query = container.GetMigrationQuery("System.Data.SqlClient");

            Assert.AreEqual("CREATE DATABASE [db2];\r\rDROP DATABASE IF EXISTS [db1];\r\rDROP TABLE IF EXISTS [dbo].[posts];\r\rDROP TABLE IF EXISTS [dbo].[users];\r\rCREATE TABLE [dbo].[users] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[username] NVARCHAR(255) NOT NULL\r);\r\rCREATE TABLE [dbo].[posts] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[title] NVARCHAR(255) NOT NULL,\r\t[user_id] INT NOT NULL\r);\r\rSET IDENTITY_INSERT [dbo].[users] ON;\rINSERT INTO [dbo].[users] ([id],[username]) VALUES (1,'user 1');\rSET IDENTITY_INSERT [dbo].[users] OFF;\r\rSET IDENTITY_INSERT [dbo].[posts] ON;\rINSERT INTO [dbo].[posts] ([id],[title],[user_id]) VALUES (1,'post 1',1);\rSET IDENTITY_INSERT [dbo].[posts] OFF;\r\rALTER TABLE [dbo].[users] ADD PRIMARY KEY ([id]);\r\rALTER TABLE [dbo].[posts] ADD PRIMARY KEY ([id]);\r\rALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\r\rALTER TABLE [dbo].[table2] ADD [column1] NVARCHAR(255) NOT NULL;\r\rDROP TABLE IF EXISTS [dbo].[table3];\r", query);
        }

        [TestMethod]
        public void TestUpdateOnlyFor()
        {
            var service = new FakeDb();

            A.UpIsCalled = false;
            A.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, container.HasCreateTableCommands);

            var recognizedFile = new RecognizedMigrationFile(typeof(A), "A", "A");

            container.RunMigrationAndUpdateDatabase(recognizedFile, "", "System.Data.SqlClient");

            Assert.IsTrue(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, container.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateOnlyFor_WithDown()
        {
            var service = new FakeDb();

            A.UpIsCalled = false;
            A.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, container.HasCreateTableCommands);

            var recognizedFile = new RecognizedMigrationFile(typeof(A), "A", "A");

            container.RunMigrationAndUpdateDatabase(recognizedFile, "", "System.Data.SqlClient",null, MigrationDirection.Down);

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsTrue(A.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, container.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateAll()
        {
            var service = new FakeDb();

            A.UpIsCalled = false;
            A.DownIsCalled = false;
            B.UpIsCalled = false;
            B.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, container.HasCreateTableCommands);

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(A), "A", "A"),
                new RecognizedMigrationFile(typeof(B), "B", "B")
            };

            container.DoMigrations(files, "", "System.Data.SqlClient");

            Assert.IsTrue(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsTrue(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, container.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateAll_WithDown()
        {
            var service = new FakeDb();

            A.UpIsCalled = false;
            A.DownIsCalled = false;
            B.UpIsCalled = false;
            B.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, container.HasCreateTableCommands);

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(A), "A", "A"),
                new RecognizedMigrationFile(typeof(B), "B", "B")
            };

            container.DoMigrations(files, "", "System.Data.SqlClient", null, MigrationDirection.Down);

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsTrue(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsTrue(B.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, container.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestSeedAll_RunLast()
        {
            var service = new FakeDb();

            _1_Seed.IsCalled = false;
            _2_Seed.IsCalled = false;

            Assert.IsFalse(_1_Seed.IsCalled);
            Assert.IsFalse(_2_Seed.IsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"));

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(_1_Seed), "_1_Seed", "Seed", "_1"),
                new RecognizedMigrationFile(typeof(_2_Seed), "_2_Seed", "Seed", "_2_")
            };

            container.DoSeedAll(files, "", "System.Data.SqlClient");

            Assert.IsFalse(_1_Seed.IsCalled);
            Assert.IsTrue(_2_Seed.IsCalled);
        }

        [TestMethod]
        public void TestUpdateAll_RunLast()
        {
            var service = new FakeDb();

            _1_Mig.UpIsCalled = false;
            _2_Mig.UpIsCalled = false;

            Assert.IsFalse(_1_Mig.UpIsCalled);
            Assert.IsFalse(_2_Mig.UpIsCalled);

            var container = new CommandContainer(new MigrationAssemblyService(), service);

            container
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1"),
                new RecognizedMigrationFile(typeof(_2_Mig), "_2_Mig", "Mig", "_2_")
            };

            container.DoMigrations(files, "", "System.Data.SqlClient");

            Assert.IsFalse(_1_Mig.UpIsCalled);
            Assert.IsTrue(_2_Mig.UpIsCalled);
        }
    }

    public class _1_Seed : Seeder
    {
        public static bool IsCalled { get; set; }

        public override void Run()
        {
            IsCalled = true;
        }
    }

    public class _2_Seed : Seeder
    {
        public static bool IsCalled { get; set; }

        public override void Run()
        {
            IsCalled = true;
        }
    }

    public class _1_Mig : Migration
    {
        public static bool UpIsCalled { get; set; }

        public override void Up()
        {
            UpIsCalled = true;
        }
    }

    public class _2_Mig : Migration
    {
        public static bool UpIsCalled { get; set; }

        public override void Up()
        {
            UpIsCalled = true;
        }
    }

    public class FakeDb : IDbService
    {
        public string Query { get; set; }
        public bool IsCalled { get; set; }

        public DbConnection Connection => throw new NotImplementedException();

        public void Close()
        {

        }

        public IDbService CreateConnection(string connectionString, string providerName)
        {

            return this;
        }

        public int Execute(string sql)
        {
            this.IsCalled = true;
            this.Query = sql;
            return 1;
        }

        public Task ExecuteAsync(string sql)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            throw new NotImplementedException();
        }

        public bool IsOpen()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            
        }

        public Task OpenAsync()
        {
            throw new NotImplementedException();
        }

        public List<Dictionary<string, object>> ReadAll(string sql)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> ReadOne(string sql)
        {
            throw new NotImplementedException();
        }
    }
}
