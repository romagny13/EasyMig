using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Db.MySqlClient;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace EasyMigTest.Db.MySqlClient
{
    [TestClass]
    public class MySqlDbServiceTest
    {
        private string connectionString = "server=localhost;database=db1;uid=root";

        public MySqlDbService GetService()
        {
           var service = new MySqlDbService();
            service.CreateConnection(connectionString);
            service.Open();
            return service;
        }


        [TestMethod]
        public void TestMySqlDbService()
        {
            TestExecute();

            TestExecuteScalar();

            TestReadOne();

            TestReadOne_WitParameter();

            TestReadOne_WithNullable_ReturnsNull();

            TestReadAll();

            TestCreateProcedure();
        }

        public void TestExecute()
        {

            var query = @"DROP TABLE IF EXISTS `db_users`;
                                    CREATE TABLE `db_users` (
                          `id` int(11) NOT NULL,
                          `username` varchar(255) NOT NULL,
                          `age` int(11) DEFAULT NULL
                        ) ENGINE = MyISAM DEFAULT CHARSET = latin1;

                        INSERT INTO `db_users` (`id`, `username`, `age`) VALUES
                        (1, 'user1', 20),
                        (2, 'user2', 30),
                        (3, 'user3', NULL);

                        ALTER TABLE `db_users`
                          ADD PRIMARY KEY(`id`);

                        ALTER TABLE `db_users`
                          MODIFY `id` int(11) UNSIGNED NOT NULL AUTO_INCREMENT; ";


            var service = this.GetService();

            var result = service.Execute(query);

            Assert.IsTrue(result > 1);
        }


        public void TestExecuteScalar()
        {
            var service = this.GetService();

            var result = service.ExecuteScalar("select count(*) from db_users");

            Assert.AreEqual((Int64)3, (Int64)result);
        }

        public void TestReadOne()
        {
            var service = this.GetService();

            var result =  service.ReadOne("select * from db_users where id=2");

            Assert.IsNotNull(result);
            Assert.AreEqual((uint)2, (uint)result["id"]);
            Assert.AreEqual("user2", (string)result["username"]);
            Assert.AreEqual(30, (int)result["age"]);
        }

        public void TestReadOne_WitParameter()
        {
            var service = this.GetService();

            var result = service.ReadOne("select * from db_users where id=@id", new List<MySqlParameter> {
                new MySqlParameter { ParameterName="@id", Value=2 }
            });

            Assert.IsNotNull(result);
            Assert.AreEqual((uint)2, (uint)result["id"]);
            Assert.AreEqual("user2", (string)result["username"]);
            Assert.AreEqual(30, (int)result["age"]);
        }

        public void TestReadOne_WithNullable_ReturnsNull()
        {
            var service = this.GetService();

            var result = service.ReadOne("select * from db_users where id=3");

            Assert.AreEqual((uint)3, (uint)result["id"]);
            Assert.AreEqual("user3", (string)result["username"]);
            Assert.AreEqual(null, result["age"]);
        }

        public void TestReadAll()
        {
            var service = this.GetService();

            var users = service.ReadAll("select * from db_users");

            Assert.AreEqual(3, users.Count);

            Assert.AreEqual((uint)1, (uint)users[0]["id"]);
            Assert.AreEqual("user1", (string)users[0]["username"]);
            Assert.AreEqual(20, (int)users[0]["age"]);

            Assert.AreEqual((uint)2, (uint)users[1]["id"]);
            Assert.AreEqual("user2", (string)users[1]["username"]);
            Assert.AreEqual(30, (int)users[1]["age"]);

            Assert.AreEqual((uint)3, (uint)users[2]["id"]);
            Assert.AreEqual("user3", (string)users[2]["username"]);
            Assert.AreEqual(null, users[2]["age"]); // nullable 
        }

        public void TestCreateProcedure()
        {
            bool failed = false;
            try
            {
                var query = @"DROP PROCEDURE IF EXISTS `get_db_user`;
                            CREATE DEFINER=`root`@`localhost` PROCEDURE `get_db_user`(IN `p_id` INT)
                            BEGIN
                            select * from db_users where id=p_id;
                            END";

                var service = this.GetService();

                var result = service.Execute(query);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }
    }
}
