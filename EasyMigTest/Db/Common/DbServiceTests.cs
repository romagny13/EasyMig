using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Db.Common;
using System.Collections.Generic;

namespace EasyMigTest.Db.Common
{
    [TestClass]
    public class DbServiceTests
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\romag\Documents\Visual Studio 2017\Projects\experimental\EasyMigLib\EasyMigTest\dbTest.mdf;Integrated Security=True;Connect Timeout=30";
        private string providerName = "System.Data.SqlClient";


        public DbService GetService()
        {
            var service = new DbService();
            service.CreateConnection(connectionString, providerName);
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

            var query = @"DROP TABLE IF EXISTS [dbo].[db_users];
                        CREATE TABLE [dbo].[db_users] (
	                        [id] INT NOT NULL IDENTITY(1,1),
	                        [username] NVARCHAR(255) NOT NULL,
	                        [age] INT NULL,
                        );

                        SET IDENTITY_INSERT [dbo].[db_users] ON;
                        INSERT INTO [dbo].[db_users] ([id],[username],[age]) VALUES (1,'user1',20);
                        INSERT INTO [dbo].[db_users] ([id],[username],[age]) VALUES (2,'user2',30);
                        INSERT INTO [dbo].[db_users] ([id],[username],[age]) VALUES (3,'user3',NULL);
                        SET IDENTITY_INSERT [dbo].[db_users] OFF;

                        ALTER TABLE [dbo].[db_users] ADD PRIMARY KEY ([id]);";


            var service = this.GetService();

            var result = service.Execute(query);

            Assert.IsTrue(result > 1);
        }


        public void TestExecuteScalar()
        {
            var service = this.GetService();

            var result = service.ExecuteScalar("select count(*) from db_users");

            Assert.AreEqual(3, (int)result);
        }

        public void TestReadOne()
        {
            var service = this.GetService();

            var result = service.ReadOne("select * from db_users where id=2");

            Assert.IsNotNull(result);
            Assert.AreEqual(2, (int)result["id"]);
            Assert.AreEqual("user2", (string)result["username"]);
            Assert.AreEqual(30, (int)result["age"]);
        }

        public void TestReadOne_WitParameter()
        {
            var service = this.GetService();

            var result = service.ReadOne("select * from db_users where id=@id", new List<DbServiceParameter>
            {
                new DbServiceParameter { ParameterName="@id", Value=2 }
            });

            Assert.IsNotNull(result);
            Assert.AreEqual(2, (int)result["id"]);
            Assert.AreEqual("user2", (string)result["username"]);
            Assert.AreEqual(30, (int)result["age"]);
        }

        public void TestReadOne_WithNullable_ReturnsNull()
        {
            var service = this.GetService();

            var result = service.ReadOne("select * from db_users where id=3");

            Assert.AreEqual(3, result["id"]);
            Assert.AreEqual("user3", (string)result["username"]);
            Assert.AreEqual(null, result["age"]);
        }

        public void TestReadAll()
        {
            var service = this.GetService();

            var users = service.ReadAll("select * from db_users");

            Assert.AreEqual(3, users.Count);

            Assert.AreEqual(1, (int)users[0]["id"]);
            Assert.AreEqual("user1", (string)users[0]["username"]);
            Assert.AreEqual(20, (int)users[0]["age"]);

            Assert.AreEqual(2, (int)users[1]["id"]);
            Assert.AreEqual("user2", (string)users[1]["username"]);
            Assert.AreEqual(30, (int)users[1]["age"]);

            Assert.AreEqual(3, (int)users[2]["id"]);
            Assert.AreEqual("user3", (string)users[2]["username"]);
            Assert.AreEqual(null, users[2]["age"]); // nullable 
        }

        public void TestCreateProcedure()
        {
            bool failed = false;
            try
            {
                var service = this.GetService();

                var dropQuery = @"DROP PROCEDURE IF EXISTS [dbo].[get_db_users]";

                service.Execute(dropQuery);

                var query = @"CREATE PROCEDURE [dbo].[get_db_users] 
                            AS
                            BEGIN
                            select * from users
                            END";

                var result = service.Execute(query);
            }
            catch (Exception e)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }
    }
}
