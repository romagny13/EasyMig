using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.MySqlClient;
using EasyMigLib.Db.MySqlClient;
using EasyMigLib.Information.MySqlClient;

namespace EasyMigTest.SqlClient
{
    [TestClass]
    public class MySqlInformationTest
    {
        public static string connectionString = @"server=localhost;database=db1;uid=root";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var service = new MySqlDbService();
            service.CreateConnection(connectionString);
            service.Open();
            service.Execute(@"
                    CREATE DATABASE IF NOT EXISTS `db1` DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci;
                                USE `db1`;

                    DROP TABLE IF EXISTS `info_users`;
                    CREATE TABLE `info_users` (
                      `id` int(11) UNSIGNED NOT NULL,
                      `username` varchar(255) NOT NULL,
                      `age` int(11) DEFAULT NULL
                    ) ENGINE = MyISAM DEFAULT CHARSET = latin1;


                    INSERT INTO `info_users` (`id`, `username`, `age`) VALUES
                    (1, 'user1', 20),
                    (2, 'user2', 30),
                    (3, 'user3', NULL);


                    ALTER TABLE `info_users`
                      ADD PRIMARY KEY(`id`);

                    ALTER TABLE `info_users`
                      MODIFY `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT = 4; ");

            service.Close();
        }

        public MySqlInformation GetService()
        {
            return new MySqlInformation();
        }

        [TestMethod]
        public void TestDatabaseExists_ReturnsTrue()
        {
            var service = this.GetService();

            Assert.IsTrue(service.DatabaseExists("db1", connectionString));
        }

        [TestMethod]
        public void TestDatabaseExists_ReturnsFalse()
        {
            var service = this.GetService();

            Assert.IsFalse(service.DatabaseExists("not_found", connectionString));
        }

        [TestMethod]
        public void TestTableExists_ReturnsTrue()
        {
            var service = this.GetService();

            Assert.IsTrue(service.TableExists("db1", "info_users", connectionString));
        }

        [TestMethod]
        public void TestTableExists_ReturnsFalse()
        {
            var service = this.GetService();

            Assert.IsFalse(service.TableExists("db1", "not_found", connectionString));
        }

        [TestMethod]
        public void TestColumnExists_ReturnsTrue()
        {
            var service = this.GetService();

            Assert.IsTrue(service.ColumnExists("db1", "info_users", "username", connectionString));
        }

        [TestMethod]
        public void TestColumnExists_ReturnsFalse()
        {
            var service = this.GetService();

            Assert.IsFalse(service.ColumnExists("db1", "info_users", "not_found", connectionString));
        }


        [TestMethod]
        public void TestGetTable()
        {
            var service = this.GetService();

            var result = service.GetTable("db1", "info_users", connectionString);


            Assert.AreEqual("info_users", result.Table["TABLE_NAME"]);

            Assert.AreEqual(3, result.Columns.Count);
            Assert.AreEqual("id", result.Columns["id"]["COLUMN_NAME"]);
            Assert.AreEqual("username", result.Columns["username"]["COLUMN_NAME"]);
            Assert.AreEqual("age", result.Columns["age"]["COLUMN_NAME"]);

            Assert.AreEqual(1, result.PrimaryKeys.Count);
            Assert.AreEqual("id", result.PrimaryKeys["id"]["COLUMN_NAME"]);
        }

        [TestMethod]
        public void TestGetTableRows()
        {
            var service = this.GetService();

            var result = service.GetTableRows("info_users", connectionString);

            Assert.AreEqual(3, result.Count);

            Assert.AreEqual((uint)1, (uint)result[0]["id"]);
            Assert.AreEqual("user1", (string)result[0]["username"]);
            Assert.AreEqual(20, (int)result[0]["age"]);

            Assert.AreEqual((uint)2, (uint)result[1]["id"]);
            Assert.AreEqual("user2", (string)result[1]["username"]);
            Assert.AreEqual(30, (int)result[1]["age"]);

            Assert.AreEqual((uint)3, (uint)result[2]["id"]);
            Assert.AreEqual("user3", (string)result[2]["username"]);
            Assert.AreEqual(null, result[2]["age"]);
        }
    }
}
