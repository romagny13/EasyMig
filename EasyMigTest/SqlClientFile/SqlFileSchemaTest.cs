using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Db.Common;
using System.Collections.Generic;
using EasyMigLib.MigrationReflection;
using EasyMigLib.SqlClient;
using EasyMigLib.Query.SqlClient;
using EasyMigLib.Commands;
using EasyMigTest.MySqlClient;
using System.Threading.Tasks;
using EasyMigLibTest.Services;
using EasyMigTest.SqlClient;
using EasyMigLib;
using EasyMigLib.SqlClientAttachedDbFile;

namespace EasyMigTest.SqlClientFile
{
    [TestClass]
    public class SqlFileSchemaTest
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\experimental\EasyMigLib\EasyMigTest\dbTest.mdf;Integrated Security=True;Connect Timeout=30";

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

            var commands = new CommandContainer();
            commands.DropTable(name);

            try
            {
                commands.DropTable("table2");
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

            var commands = new CommandContainer();

            commands.CreateDatabase(name);

            Assert.IsTrue(commands.HasCreateDatabaseCommand(name));

            var result = commands.GetCreateDatabaseCommand(name);

            Assert.AreEqual(typeof(CreateDatabaseCommand), result.GetType());
            Assert.AreEqual(name, result.DatabaseName);
        }

        // drop database

        [TestMethod]
        public void TestDropDatabaseCommand()
        {
            var name = "db1";

            var commands = new CommandContainer();

            commands.DropDatabase(name);

            Assert.IsTrue(commands.HasDropDatabaseCommand(name));

            var result = commands.GetDropDatabaseCommand(name);

            Assert.AreEqual(typeof(DropDatabaseCommand), result.GetType());
            Assert.AreEqual(name, result.DatabaseName);
        }

        // create table

        [TestMethod]
        public void TestCreateTableCommand()
        {
            var name = "table1";

            var commands = new CommandContainer();

            commands.CreateTable(name);

            Assert.IsTrue(commands.HasCreateTableCommand(name));

            var result = commands.GetCreateTableCommand(name);

            Assert.AreEqual(typeof(CreateTableCommand), result.GetType());
            Assert.AreEqual(name, result.TableName);
        }



        // alter table

        [TestMethod]
        public void TestAlterTableCommand()
        {
            var name = "table1";

            var commands = new CommandContainer();

            commands.AlterTable(name)
                .AddColumn("column1");

            Assert.IsTrue(commands.HasAlterTableCommand(name));

            var result = commands.GetAlterTableCommand(name);

            Assert.AreEqual(name, result.TableName);
            Assert.AreEqual(true, result.HasAddColumnCommands);
        }


        [TestMethod]
        public void TestAlterTableCommand_ReturnsOldTable()
        {
            var name = "table1";

            var commands = new CommandContainer();
            commands.AlterTable(name)
                   .AddColumn("column1");

            Assert.IsTrue(commands.HasAlterTableCommand(name));

            commands.AlterTable(name)
                 .AddColumn("column2");

            Assert.IsTrue(commands.GetAlterTableCommand(name).HasAddColumnCommand("column1"));
            Assert.IsTrue(commands.GetAlterTableCommand(name).HasAddColumnCommand("column2"));
        }

        // drop table

        [TestMethod]
        public void TestDropTableCommand()
        {
            var name = "table1";
            var commands = new CommandContainer();
            commands.DropTable(name);

            Assert.IsTrue(commands.HasDropTableCommand(name));

            var result = commands.GetDropTableCommand(name);

            Assert.AreEqual(name, result.TableName);
        }

        // stored procedure query

        [TestMethod]
        public void TestGetStoredProcedureQuery()
        {
            var commands = new CommandContainer();

            commands.CreateStoredProcedure("proc1")
                .AddParameter("@id", ColumnType.Int())
                .SetBody("select * from users where id=@id");

            commands.CreateStoredProcedure("proc2")
               .SetBody("select * from users");

            var container = new SqlFileSchema(commands);
            var query = container.GetStoredProcedureQuery();

            Assert.AreEqual("DROP PROCEDURE IF EXISTS [dbo].[proc1]\rGO\rCREATE PROCEDURE [dbo].[proc1] @id INT\rAS\rBEGIN\rselect * from users where id=@id;\rEND\rGO\rDROP PROCEDURE IF EXISTS [dbo].[proc2]\rGO\rCREATE PROCEDURE [dbo].[proc2] \rAS\rBEGIN\rselect * from users;\rEND\rGO", query);
        }

        // seed

        [TestMethod]
        public void TestSeed()
        {
            var tableName = "table1";

            var commands = new CommandContainer();
            commands.SeedTable(tableName)
                .Insert(new Dictionary<string, object> { { "id", 1 }, { "column1", "value 1" } })
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            Assert.IsTrue(commands.HasSeedTable(tableName));

            var seedTable = commands.GetSeedTable(tableName);

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

            var commands = new CommandContainer();
            commands.SeedTable(tableName)
                .Insert(new Dictionary<string, object> { { "id", 1 }, { "column1", "value 1" } })
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            Assert.IsTrue(commands.HasSeedTable(tableName));

            var seedTable = commands.GetSeedTable(tableName);

            Assert.AreEqual(2, seedTable.RowCount);

            commands.SeedTable(tableName)
                .Insert(SeedData.New.Set("id", 3).Set("column1", "value 3"));

            var container = new SqlFileSchema(commands);
            Assert.AreEqual(3, commands.GetSeedTable(tableName).RowCount);

            var row = seedTable.GetRow(2);
            Assert.AreEqual(3, row["id"]);
            Assert.AreEqual("value 3", row["column1"]);
        }


        // get seed query

        [TestMethod]
        public void TestGetSeedQuery()
        {
            var commands = new CommandContainer();
            commands.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            commands.SeedTable("table2")
               .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

            var container = new SqlFileSchema(commands);
            var query = container.GetSeedQuery();

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value 1');\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value 2');\r\rINSERT INTO [dbo].[table2] ([id],[columna]) VALUES (1,'value a');\r", query);
        }


        [TestMethod]
        public void TestGetSeedOnlyFor_SeederAndDbServiceCalled()
        {
            var service = new SqlFakeDbService();

            SeederA.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(service.IsCalled);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, new CommandContainer());

            var recognizedFile = new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA");

            container.DoSeedOne(recognizedFile,"");

            Assert.IsTrue(SeederA.IsCalled);
            Assert.IsTrue(service.IsCalled);
        }

        [TestMethod]
        public void TestSeederIsClear()
        {
            var service = new SqlFakeDbService();
            var commands = new CommandContainer();
            commands.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            commands.SeedTable("table2")
               .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

            Assert.AreEqual(true, commands.HasSeedCommands);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var recognizedFile = new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA");
            container.DoSeedOne(recognizedFile, "");

            Assert.AreEqual("INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value 1');\rINSERT INTO [dbo].[table1] ([id],[column1]) VALUES (2,'value 2');\r\rINSERT INTO [dbo].[table2] ([id],[columna]) VALUES (1,'value a');\r", service.Query);

            Assert.AreEqual(false, commands.HasSeedCommands);
        }

        [TestMethod]
        public void TestGetSeedAll_SeederAndDbServiceCalled()
        {
            var service = new SqlFakeDbService();

            SeederA.IsCalled = false;
            SeederB.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(SeederB.IsCalled);
            Assert.IsFalse(service.IsCalled);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, new CommandContainer());

            var files = new List<RecognizedMigrationFile> {
                new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA"),
                new RecognizedMigrationFile(typeof(SeederB), "SeederB", "SeederB")
            };

            container.DoSeedAll(files, connectionString);

            Assert.IsTrue(SeederA.IsCalled);
            Assert.IsTrue(SeederB.IsCalled);
            Assert.IsTrue(service.IsCalled);
        }

        [TestMethod]
        public void TestGetSeedAll_SeedersAreClear()
        {
            var service = new SqlFakeDbService();

            SeederA.IsCalled = false;
            SeederB.IsCalled = false;

            Assert.IsFalse(SeederA.IsCalled);
            Assert.IsFalse(SeederB.IsCalled);
            Assert.IsFalse(service.IsCalled);


            var commands = new CommandContainer();
            commands.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value 2"));

            commands.SeedTable("table2")
   .Insert(SeedData.New.Set("id", 1).Set("columna", "value a"));

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);


            Assert.AreEqual(true, commands.HasSeedCommands);


            var files = new List<RecognizedMigrationFile> {
                new RecognizedMigrationFile(typeof(SeederA), "SeederA", "SeederA"),
                new RecognizedMigrationFile(typeof(SeederB), "SeederB", "SeederB")
            };

            container.DoSeedAll(files, "");

            Assert.AreEqual(false, commands.HasSeedCommands);
        }


        //[TestMethod]
        //public void TestGetMigrationQuery()
        //{
        //    var container = new SqlFileSchema();

        //    container.DropDatabase("db1");
        //    container.CreateDatabase("db2");

        //    container
        //        .CreateTable("users")
        //        .AddPrimaryKey("id")
        //        .AddColumn("username")
        //        .Insert(SeedData.New.Set("id",1).Set("username","user 1"));

        //   container
        //        .CreateTable("posts")
        //        .AddPrimaryKey("id")
        //        .AddColumn("title")
        //        .AddForeignKey("user_id","users","id")
        //        .Insert(SeedData.New.Set("id", 1).Set("title", "post 1").Set("user_id",1));

        //    container.AlterTable("table2").AddColumn("column1");

        //    container.DropTable("table3");

        //    var query = container.GetMigrationQuery();

        //    Assert.AreEqual("CREATE DATABASE [db2];\r\rDROP DATABASE IF EXISTS [db1];\r\rDROP TABLE IF EXISTS [dbo].[posts];\r\rDROP TABLE IF EXISTS [dbo].[users];\r\rCREATE TABLE [dbo].[users] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[username] NVARCHAR(255) NOT NULL\r);\r\rCREATE TABLE [dbo].[posts] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[title] NVARCHAR(255) NOT NULL,\r\t[user_id] INT NOT NULL\r);\r\rSET IDENTITY_INSERT [dbo].[users] ON;\rINSERT INTO [dbo].[users] ([id],[username]) VALUES (1,'user 1');\rSET IDENTITY_INSERT [dbo].[users] OFF;\r\rSET IDENTITY_INSERT [dbo].[posts] ON;\rINSERT INTO [dbo].[posts] ([id],[title],[user_id]) VALUES (1,'post 1',1);\rSET IDENTITY_INSERT [dbo].[posts] OFF;\r\rALTER TABLE [dbo].[users] ADD PRIMARY KEY ([id]);\r\rALTER TABLE [dbo].[posts] ADD PRIMARY KEY ([id]);\r\rALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\r\rALTER TABLE [dbo].[table2] ADD [column1] NVARCHAR(255) NOT NULL;\r\rDROP TABLE IF EXISTS [dbo].[table3];\r", query);
        //}

        [TestMethod]
        public void TestUpdateOnlyFor()
        {
            var service = new SqlFakeDbService();

            A.UpIsCalled = false;
            A.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);

            var commands = new CommandContainer();
            commands
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, commands.HasCreateTableCommands);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var recognizedFile = new RecognizedMigrationFile(typeof(A), "A", "A");

            container.RunMigrationAndUpdateDatabase(recognizedFile, "");

            Assert.IsTrue(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, commands.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateOnlyFor_WithDown()
        {
            var service = new SqlFakeDbService();

            A.UpIsCalled = false;
            A.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);

            var commands = new CommandContainer();
            commands
             .CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, commands.HasCreateTableCommands);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var recognizedFile = new RecognizedMigrationFile(typeof(A), "A", "A");

            container.RunMigrationAndUpdateDatabase(recognizedFile, "", MigrationDirection.Down);

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsTrue(A.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, commands.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateAll()
        {
            var service = new SqlFakeDbService();

            A.UpIsCalled = false;
            A.DownIsCalled = false;
            B.UpIsCalled = false;
            B.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);

            var commands = new CommandContainer();
            commands.CreateTable("users")
             .AddPrimaryKey("id")
             .AddColumn("username")
             .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, commands.HasCreateTableCommands);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(A), "A", "A"),
                new RecognizedMigrationFile(typeof(B), "B", "B")
            };

            container.DoMigrations(files, "");

            Assert.IsTrue(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsTrue(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, commands.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestUpdateAll_WithDown()
        {
            var service = new SqlFakeDbService();

            A.UpIsCalled = false;
            A.DownIsCalled = false;
            B.UpIsCalled = false;
            B.DownIsCalled = false;

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsFalse(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsFalse(B.DownIsCalled);

            var commands = new CommandContainer();
            commands
              .CreateTable("users")
              .AddPrimaryKey("id")
              .AddColumn("username")
              .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            Assert.AreEqual(true, commands.HasCreateTableCommands);

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(A), "A", "A"),
                new RecognizedMigrationFile(typeof(B), "B", "B")
            };

            container.DoMigrations(files, "", MigrationDirection.Down);

            Assert.IsFalse(A.UpIsCalled);
            Assert.IsTrue(A.DownIsCalled);
            Assert.IsFalse(B.UpIsCalled);
            Assert.IsTrue(B.DownIsCalled);
            Assert.IsTrue(service.IsCalled);

            Assert.AreEqual(false, commands.HasCreateTableCommands);
        }

        [TestMethod]
        public void TestSeedAll_RunLast()
        {
            var service = new SqlFakeDbService();

            _1_Seed.IsCalled = false;
            _2_Seed.IsCalled = false;

            Assert.IsFalse(_1_Seed.IsCalled);
            Assert.IsFalse(_2_Seed.IsCalled);


            var commands = new CommandContainer();
            commands.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value 1"));

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);

            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(_1_Seed), "_1_Seed", "Seed", "_1"),
                new RecognizedMigrationFile(typeof(_2_Seed), "_2_Seed", "Seed", "_2_")
            };

            container.DoSeedAll(files, "");

            Assert.IsFalse(_1_Seed.IsCalled);
            Assert.IsTrue(_2_Seed.IsCalled);
        }

        [TestMethod]
        public void TestUpdateAll_RunLast()
        {
            var service = new SqlFakeDbService();

            _1_Mig.UpIsCalled = false;
            _2_Mig.UpIsCalled = false;

            Assert.IsFalse(_1_Mig.UpIsCalled);
            Assert.IsFalse(_2_Mig.UpIsCalled);

            var commands = new CommandContainer();
            commands.CreateTable("users")
            .AddPrimaryKey("id")
            .AddColumn("username")
            .Insert(SeedData.New.Set("id", 1).Set("username", "user 1"));

            var container = new SqlFileSchema(new MigrationAssemblyService(), new SqlQueryService(), service, commands);


            var files = new List<RecognizedMigrationFile> {
               new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1"),
                new RecognizedMigrationFile(typeof(_2_Mig), "_2_Mig", "Mig", "_2_")
            };

            container.DoMigrations(files, "");

            Assert.IsFalse(_1_Mig.UpIsCalled);
            Assert.IsTrue(_2_Mig.UpIsCalled);
        }
    }
}
