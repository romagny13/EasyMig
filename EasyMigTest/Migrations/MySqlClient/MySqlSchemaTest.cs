using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using EasyMigLib.Db.MySqlClient;
using EasyMigLib;
using EasyMigLib.MigrationReflection;
using EasyMigLib.Query.MySqlClient;
using EasyMigLib.Commands;
using EasyMigLibTest.Services;
using EasyMigLib.Migrations.MySqlClient;

namespace EasyMigTest
{
    [TestClass]
    public class MySqlSchemaTest
    {
        public static string connectionString = @"server=localhost;database=db1;uid=root";
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

        // create and use database

        [TestMethod]
        public void TestCreateAndUseDatabaseCommand()
        {
            var name = "db1";

            var commands = new CommandContainer();

            commands.CreateAndUseDatabase(name);

            Assert.IsTrue(commands.HasCreateAndUseDatabaseCommand(name));

            var result = commands.GetCreateAndUseDatabaseCommand(name);

            Assert.AreEqual(typeof(CreateAndUseDatabaseCommand), result.GetType());
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

            var container = new MySqlSchema(commands);
            Assert.AreEqual(3, commands.GetSeedTable(tableName).RowCount);

            var row = seedTable.GetRow(2);
            Assert.AreEqual(3, row["id"]);
            Assert.AreEqual("value 3", row["column1"]);
        }

        // Get stored procedures query

        [TestMethod]
        public void TestGetStoredProceduresQuery()
        {
            var commands = new CommandContainer();
            commands.CreateStoredProcedure("p1").SetBody("select * from users");
            commands.CreateStoredProcedure("p2").SetBody("select * from users");

            var container = new MySqlSchema(commands);

            var result = container.GetCreateStoredProceduresQuery();

            Assert.AreEqual("DELIMITER $$\r\rDROP PROCEDURE IF EXISTS `p1`$$CREATE PROCEDURE `p1`()\rBEGIN\rselect * from users;\rEND$$\r\rDROP PROCEDURE IF EXISTS `p2`$$CREATE PROCEDURE `p2`()\rBEGIN\rselect * from users;\rEND$$\r\rDELIMITER ;\r", result);
        }


        [TestMethod]
        public void TestDbServiceIsCalledOnQueries()
        {
            var dbService = new MySqlFakeDbService();
            var container = new MySqlSchema(new MigrationAssemblyService(),
                  new MySqlQueryService(),
                  dbService,
                  new MySqlQuerySplitter(),
                  new CommandContainer());

            container.ExecuteQueries(new string[] { "q1", "q2" });

            Assert.AreEqual(2, dbService.Results.Count);
            Assert.AreEqual("q1", dbService.Results[0]);
            Assert.AreEqual("q2", dbService.Results[1]);
        }


        [TestMethod]
        public void TestSeederIsCalled()
        {
            var container = new MySqlSchema(new CommandContainer());

            _1_Seed.IsCalled = false;

            var file = new RecognizedMigrationFile(typeof(_1_Seed), "_1_Seed", "Seed", "_1");
            container.RunSeeder(file);

            Assert.IsTrue(_1_Seed.IsCalled);
        }

        [TestMethod]
        public void TestMigrationIsCalled()
        {
            var container = new MySqlSchema(new CommandContainer());

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
            var container = new MySqlSchema(new CommandContainer());

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
            var container = new MySqlSchema(new CommandContainer());


            var files = new List<RecognizedMigrationFile>{
                new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1"),
                new RecognizedMigrationFile(typeof(_2_Mig), "_2_Mig", "Mig", "_2"),
            };

            var result = container.GetLast(files);

            Assert.AreEqual("_2",result.Version);
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
        public static bool DownIsCalled { get; set; }

        public override void Up()
        {
            UpIsCalled = true;
        }

        public override void Down()
        {
            DownIsCalled = true;
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

    public class MySqlFakeDbService : IMySqlDbService
    {
        public bool IsCalled { get; set; }
        public List<string> Results = new List<string>();


        public MySqlConnection Connection => throw new NotImplementedException();

        public void Close()
        {

        }

        public IMySqlDbService CreateConnection(string connectionString)
        {
            return this;
        }

        public int Execute(string sql, List<MySqlParameter> parameters = null)
        {
            this.Results.Add(sql);
            this.IsCalled = true;
            return 1;
        }

        public Task ExecuteAsync(string sql, List<MySqlParameter> parameters = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(string sql, List<MySqlParameter> parameters = null)
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

        public List<Dictionary<string, object>> ReadAll(string sql, List<MySqlParameter> parameters = null)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, object> ReadOne(string sql, List<MySqlParameter> parameters = null)
        {
            throw new NotImplementedException();
        }
    }
}
