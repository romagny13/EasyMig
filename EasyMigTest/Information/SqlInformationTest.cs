using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Db.Common;
using EasyMigLib.Information.SqlClient;

namespace EasyMigTest.MySqlClient
{
    [TestClass]
    public class SqlInformationTest
    {
        private static string connectionString = @"Server=localhost\SQLEXPRESS;Database=db1;Trusted_Connection=True;";

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            var query = @"DROP TABLE IF EXISTS [dbo].[info_users];
                        CREATE TABLE [dbo].[info_users] (
	                        [id] INT NOT NULL IDENTITY(1,1),
	                        [username] NVARCHAR(255) NOT NULL,
	                        [age] INT NULL,
                        );

                        SET IDENTITY_INSERT [dbo].[info_users] ON;
                        INSERT INTO [dbo].[info_users] ([id],[username],[age]) VALUES (1,'user1',20);
                        INSERT INTO [dbo].[info_users] ([id],[username],[age]) VALUES (2,'user2',30);
                        INSERT INTO [dbo].[info_users] ([id],[username],[age]) VALUES (3,'user3',NULL);
                        SET IDENTITY_INSERT [dbo].[info_users] OFF;

                        ALTER TABLE [dbo].[info_users] ADD PRIMARY KEY ([id]);";


            var service = new DbService();
            service.CreateConnection(connectionString, "System.Data.SqlClient");

            service.Open();

            service.Execute(query);

            service.Close();
        }

        public SqlServerDatabaseInformation GetService()
        {
          return new SqlServerDatabaseInformation();
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

            Assert.AreEqual(3,result.Columns.Count);
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

            Assert.AreEqual(1, (int)result[0]["id"]);
            Assert.AreEqual("user1", (string)result[0]["username"]);
            Assert.AreEqual(20, (int)result[0]["age"]);

            Assert.AreEqual(2, (int)result[1]["id"]);
            Assert.AreEqual("user2", (string)result[1]["username"]);
            Assert.AreEqual(30, (int)result[1]["age"]);

            Assert.AreEqual(3, (int)result[2]["id"]);
            Assert.AreEqual("user3", (string)result[2]["username"]);
            Assert.AreEqual(null, result[2]["age"]);
        }
    }
}
