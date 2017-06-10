using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Migrations.MySqlClient;
using EasyMigLib.Schema;
using EasyMigLib.Db.MySqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using EasyMigLib.MigrationReflection;
using EasyMigLib;
using EasyMigLib.Query.MySqlClient;

namespace EasyMigTest.Migrations.MySql
{
    [TestClass]
    public class MySqlExecutorTest
    {

        // queries

        // database

        [TestMethod]
        public void TestGetDropDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropDatabase("db1");
            schema.DropDatabase("db2");

            var service = new MySqlExecutor(schema);

            var result = service.GetDropDatabasesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP DATABASE IF EXISTS `db1`;\r", result[0]);
            Assert.AreEqual("DROP DATABASE IF EXISTS `db2`;\r", result[1]);
        }

        [TestMethod]
        public void TestGetCreateDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateDatabase("db1");
            schema.CreateDatabase("db2");

            var service = new MySqlExecutor(schema);

            var result = service.GetCreateDatabasesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("CREATE DATABASE `db1`;\r", result[0]);
            Assert.AreEqual("CREATE DATABASE `db2`;\r", result[1]);
        }

        [TestMethod]
        public void TestGetCreateAndUseDatabasesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateAndUseDatabase("db1");
            schema.CreateAndUseDatabase("db2");

            var service = new MySqlExecutor(schema);

            var result = service.GetCreateAndUseDatabasesQueries();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("CREATE DATABASE `db1`;\r", result[0]);
            Assert.AreEqual("USE `db1`;\r", result[1]);
            Assert.AreEqual("CREATE DATABASE `db2`;\r", result[2]);
            Assert.AreEqual("USE `db2`;\r", result[3]);
        }

        // tables

        [TestMethod]
        public void TestGetCreateTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateTable("table1")
                .AddPrimaryKey("id")
                .AddColumn("column1")
                .Insert(SeedData.New.Set("id",1).Set("column1","value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            schema.CreateTable("table2").AddPrimaryKey("id").AddColumn("column1").AddForeignKey("table1_id","table1","id");

            var service = new MySqlExecutor(schema);

            var result = service.GetCreateTablesQueries();

            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS `table2`;\r", result[0]);
            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\r", result[1]);
            Assert.AreEqual("CREATE TABLE `table1` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r", result[2]);
            Assert.AreEqual("CREATE TABLE `table2` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL,\r\t`table1_id` INT UNSIGNED NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r", result[3]);
            Assert.AreEqual("INSERT INTO `table1` (`id`,`column1`) VALUES (1,'value1');\r", result[4]);
            Assert.AreEqual("INSERT INTO `table1` (`id`,`column1`) VALUES (2,'value2');\r", result[5]);
            Assert.AreEqual("ALTER TABLE `table1` ADD PRIMARY KEY (`id`);\rALTER TABLE `table1` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r", result[6]);
            Assert.AreEqual("ALTER TABLE `table2` ADD PRIMARY KEY (`id`);\rALTER TABLE `table2` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r", result[7]);
            Assert.AreEqual("CREATE INDEX `table1_id_index` ON `table2` (`table1_id`);\rALTER TABLE `table2` ADD FOREIGN KEY (`table1_id`) REFERENCES `table1`(`id`);\r", result[8]);
        }

        [TestMethod]
        public void TestGetAlterTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.AlterTable("table1")
              .AddColumn("column1")
              .AddColumn("column2")
              .ModifyColumn("column3",ColumnType.Int())
              .ModifyColumn("column4", ColumnType.Int())
              .DropColumn("column5")
              .DropColumn("column6")
              .AddPrimaryKeyConstraint("id")
              .AddForeignKeyConstraint("table2_id","table2","id")
              .AddForeignKeyConstraint("table3_id", "table3", "id") ;

            var service = new MySqlExecutor(schema);

            var result = service.GetAlterTablesQueries();

            Assert.AreEqual(9, result.Count);
            Assert.AreEqual("ALTER TABLE `table1` ADD `column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r", result[0]);
            Assert.AreEqual("ALTER TABLE `table1` ADD `column2` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL;\r", result[1]);
            Assert.AreEqual("ALTER TABLE `table1` MODIFY COLUMN `column3` INT NOT NULL;\r", result[2]);
            Assert.AreEqual("ALTER TABLE `table1` MODIFY COLUMN `column4` INT NOT NULL;\r", result[3]);
            Assert.AreEqual("ALTER TABLE `table1` DROP COLUMN `column5`;\r", result[4]);
            Assert.AreEqual("ALTER TABLE `table1` DROP COLUMN `column6`;\r", result[5]);
            Assert.AreEqual("ALTER TABLE `table1` ADD PRIMARY KEY (`id`);\r", result[6]);
            Assert.AreEqual("ALTER TABLE `table1` ADD FOREIGN KEY (`table2_id`) REFERENCES `table2`(`id`);\r", result[7]);
            Assert.AreEqual("ALTER TABLE `table1` ADD FOREIGN KEY (`table3_id`) REFERENCES `table3`(`id`);\r", result[8]);
        }

        [TestMethod]
        public void TestGetDropTablesQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropTable("table1");
            schema.DropTable("table2");

            var service = new MySqlExecutor(schema);

            var result = service.GetDropTablesQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\r", result[0]);
            Assert.AreEqual("DROP TABLE IF EXISTS `table2`;\r", result[1]);
        }

        // stored procedures

        [TestMethod]
        public void TestGetCreateStoredProceduresQueries()
        {
            var schema = new DatabaseSchema();
            schema.CreateStoredProcedure("p1").AddInParameter("p_id",ColumnType.Int()).SetBody("select * from users where id=p_id");
            schema.CreateStoredProcedure("p2").SetBody("select * from users");

            var service = new MySqlExecutor(schema);

            var result = service.GetCreateStoredProceduresQueries();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p1`;\r", result[0]);
            Assert.AreEqual("CREATE PROCEDURE `p1`(p_id INT)\rBEGIN\rselect * from users where id=p_id;\rEND", result[1]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p2`;\r", result[2]);
            Assert.AreEqual("CREATE PROCEDURE `p2`()\rBEGIN\rselect * from users;\rEND", result[3]);
        }

        [TestMethod]
        public void TestGetDropStoredProceduresQueries()
        {
            var schema = new DatabaseSchema();
            schema.DropStoredProcedure("p1");
            schema.DropStoredProcedure("p2");

            var service = new MySqlExecutor(schema);

            var result = service.GetDropStoredProceduresQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p1`;\r", result[0]);
            Assert.AreEqual("DROP PROCEDURE IF EXISTS `p2`;\r", result[1]);
        }

        // seeders

        [TestMethod]
        public void TestGetSeedQueries()
        {
            var schema = new DatabaseSchema();
            schema.SeedTable("table1")
                .Insert(SeedData.New.Set("id", 1).Set("column1", "value1"))
                .Insert(SeedData.New.Set("id", 2).Set("column1", "value2"));

            var service = new MySqlExecutor(schema);

            var result = service.GetSeedQueries();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("INSERT INTO `table1` (`id`,`column1`) VALUES (1,'value1');\r", result[0]);
            Assert.AreEqual("INSERT INTO `table1` (`id`,`column1`) VALUES (2,'value2');\r", result[1]);
        }

        // script

        [TestMethod]
        public void TestGetCreateStoredProceduresQueries_WithScript()
        {
            var schema = new DatabaseSchema();
            schema.CreateStoredProcedure("p1").AddInParameter("p_id", ColumnType.Int()).SetBody("select * from users where id=p_id");
            schema.CreateStoredProcedure("p2").SetBody("select * from users");

            var service = new MySqlExecutor(schema);

            var result = service.GetCreateStoredProceduresQueryForScript();

            Assert.AreEqual("DELIMITER $$\r\rDROP PROCEDURE IF EXISTS `p1`$$CREATE PROCEDURE `p1`(p_id INT)\rBEGIN\rselect * from users where id=p_id;\rEND$$\r\rDROP PROCEDURE IF EXISTS `p2`$$CREATE PROCEDURE `p2`()\rBEGIN\rselect * from users;\rEND$$\r\rDELIMITER ;\r", result);
        }

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

            var service = new MySqlExecutor(schema);

            var result = service.GetMigrationQuery();

            Assert.AreEqual("DROP TABLE IF EXISTS `table2`;\r\rDROP TABLE IF EXISTS `table1`;\r\rCREATE TABLE `table1` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r\rCREATE TABLE `table2` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL,\r\t`table1_id` INT UNSIGNED NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r\rINSERT INTO `table1` (`id`,`column1`) VALUES (1,'value1');\rINSERT INTO `table1` (`id`,`column1`) VALUES (2,'value2');\r\rINSERT INTO `table2` (`id`,`column1`) VALUES (1,'value1');\r\rALTER TABLE `table1` ADD PRIMARY KEY (`id`);\rALTER TABLE `table1` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r\rALTER TABLE `table2` ADD PRIMARY KEY (`id`);\rALTER TABLE `table2` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r\rCREATE INDEX `table1_id_index` ON `table2` (`table1_id`);\rALTER TABLE `table2` ADD FOREIGN KEY (`table1_id`) REFERENCES `table1`(`id`);\r\rDELIMITER $$\r\rDROP PROCEDURE IF EXISTS `p1`$$CREATE PROCEDURE `p1`(p_id INT)\rBEGIN\rselect * from users where id=p_id;\rEND$$\r\rDROP PROCEDURE IF EXISTS `p2`$$CREATE PROCEDURE `p2`()\rBEGIN\rselect * from users;\rEND$$\r\rDELIMITER ;\r", result);
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

            var service = new MySqlExecutor(schema);

            var result = service.GetSeedQuery();

            Assert.AreEqual("INSERT INTO `table1` (`column1`) VALUES ('value1');\rINSERT INTO `table1` (`column1`) VALUES ('value2');\r\rINSERT INTO `table2` (`column1`) VALUES ('value1');\rINSERT INTO `table2` (`column1`) VALUES ('value2');\r", result);
        }

        // engine

        [TestMethod]
        public void TestEngine()
        {
            var schema = new DatabaseSchema();
            schema.CreateTable("table1")
                .AddPrimaryKey("id")
                .AddColumn("column1");

            var service = new MySqlExecutor(schema);

            var result = service.GetMigrationQueries("MyISAM");

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("DROP TABLE IF EXISTS `table1`;\r", result[0]);
            Assert.AreEqual("CREATE TABLE `table1` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`column1` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL\r) ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r", result[1]);
            Assert.AreEqual("ALTER TABLE `table1` ADD PRIMARY KEY (`id`);\rALTER TABLE `table1` MODIFY `id` INT UNSIGNED NOT NULL AUTO_INCREMENT;\r", result[2]);
        }

        // called


        [TestMethod]
        public void TestDbServiceIsCalledOnQueries()
        {
            var dbService = new MySqlFakeDbService();
            var container = new MySqlExecutor(new MigrationAssemblyService(),
                  new MySqlQueryService(),
                  dbService,
                  new DatabaseSchema());

            container.OpenConnectionAndExecuteQueries(new List<string> { "q1", "q2" },"");

            Assert.AreEqual(2, dbService.Results.Count);
            Assert.AreEqual("q1", dbService.Results[0]);
            Assert.AreEqual("q2", dbService.Results[1]);
        }


        [TestMethod]
        public void TestSeederIsCalled()
        {
            var container = new MySqlExecutor(new DatabaseSchema());

            _1_Seed.IsCalled = false;

            var file = new RecognizedMigrationFile(typeof(_1_Seed), "_1_Seed", "Seed", "_1");
            container.RunSeeder(file);

            Assert.IsTrue(_1_Seed.IsCalled);
        }

        [TestMethod]
        public void TestMigrationIsCalled()
        {
            var container = new MySqlExecutor(new DatabaseSchema());

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
            var container = new MySqlExecutor(new DatabaseSchema());

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
            var container = new MySqlExecutor(new DatabaseSchema());

            var files = new List<RecognizedMigrationFile>{
                new RecognizedMigrationFile(typeof(_1_Mig), "_1_Mig", "Mig", "_1"),
                new RecognizedMigrationFile(typeof(_2_Mig), "_2_Mig", "Mig", "_2"),
            };

            var result = container.GetLast(files);

            Assert.AreEqual("_2", result.Version);
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
