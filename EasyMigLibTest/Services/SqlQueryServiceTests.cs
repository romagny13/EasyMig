using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EasyMigLib;
using EasyMigLib.Commands;
using EasyMigLib.Services;

namespace EasyMigLibTest.Services
{
    [TestClass]
    public class SqlQueryServiceTests
    {
        public SqlQueryService GetService()
        {
            return new SqlQueryService();
        }

        // quotes

        [TestMethod]
        public void TestQuotes()
        {
            var service = this.GetService();

            Assert.AreEqual("[", service.StartQuote);
            Assert.AreEqual("]", service.EndQuote);
        }

        // table name

        [TestMethod]
        public void TestFormatTableName()
        {
            var service = this.GetService();

            var result = service.FormatTableName("table1");

            Assert.AreEqual("[dbo].[table1]", result);
        }

        [TestMethod]
        public void TestWrapWithQuotes()
        {
            var service = this.GetService();

            var result = service.WrapWithQuotes("column1");

            Assert.AreEqual("[column1]", result);
        }


        // reserved words

        [TestMethod]
        public void TestReservedWords()
        {
            var service = this.GetService();

            Assert.IsFalse(service.IsReservedWord("not found"));
            Assert.IsTrue(service.IsReservedWord("CURRENT_TIMESTAMP"));
        }

        // typed value

        [TestMethod]
        public void TestFormatValueString()
        {
            var service = this.GetService();

            Assert.AreEqual("10", service.FormatValueString(10));
            Assert.AreEqual("'my value'", service.FormatValueString("my value"));
        }

        [TestMethod]
        public void TestFormatValueString_WithDoubleQuotes()
        {
            var service = this.GetService();

            var result = service.FormatValueString("L'item");

            Assert.AreEqual("'L''item'", result);
        }

        // database

        [TestMethod]
        public void TestGetCreateDatabase()
        {
            var service = this.GetService();

            var result = service.GetCreateDatabase("db1");

            Assert.AreEqual("CREATE DATABASE [db1];\r", result);
        }


        [TestMethod]
        public void TestGetDropDatabase()
        {
            var service = this.GetService();

            var result = service.GetDropDatabase("db1");

            Assert.AreEqual("DROP DATABASE IF EXISTS [db1];\r", result);
        }

        // drop table

        [TestMethod]
        public void TestGetDropTable()
        {
            var service = this.GetService();

            var result = service.GetDropTable("table1");

            Assert.AreEqual("DROP TABLE IF EXISTS [dbo].[table1];\r", result);
        }

        // columns

        [TestMethod]
        public void TestGetAddColumn()
        {
            var service = this.GetService();

            var result = service.GetAddColumn("table1", new MigrationColumn("column1",ColumnType.VarChar(100),true));

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD [column1] NVARCHAR(100) NULL;\r", result);
        }

        [TestMethod]
        public void TestGetModifyColumn()
        {
            var service = this.GetService();

            var result = service.GetModifyColumn("table1", new MigrationColumn("column1", ColumnType.VarChar(100), true));

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ALTER COLUMN [column1] NVARCHAR(100) NULL;\r", result);
        }

        [TestMethod]
        public void TestGetDropColumn()
        {
            var service = this.GetService();

            var result = service.GetDropColumn("table1", "column1");

            Assert.AreEqual("ALTER TABLE [dbo].[table1] DROP COLUMN [column1];\r", result);
        }

        // primary key constraint

        [TestMethod]
        public void TestGetPrimaryConstraint()
        {
            var service = this.GetService();

            var result = service.GetAddPrimaryKeyConstraint("table1", new string[] { "column1" , "column2" });

            Assert.AreEqual("ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([column1],[column2]);\r", result);
        }

        // foreign key constraint

        [TestMethod]
        public void TestGetForeignConstraint()
        {
            var service = this.GetService();

            var result = service.GetAddForeignKeyConstraint("posts",new ForeignKeyColumn("user_id", ColumnType.Int(),"users","id"));

            Assert.AreEqual("ALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\r", result);
        }

        // get column

        // nullable

        [TestMethod]
        public void TestGetColumn_WithNullable()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", true);

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(255) NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithNotNullable()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", false);

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(255) NOT NULL", result);
        }

        // default

        [TestMethod]
        public void TestGetColumn_WithDefault()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.VarChar(), false, "my value");

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(255) NOT NULL DEFAULT 'my value'", result);
        }

        [TestMethod]
        public void TestGetColumn_WithDefaultInt()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Int(), false, 10);

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] INT NOT NULL DEFAULT 10", result);
        }

        // tiny int

        [TestMethod]
        public void TestGetColumn_WithTinyInt()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.TinyInt());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] TINYINT NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithTinyIntUnisgned_IgnoreUnsigned()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.TinyInt(true));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] TINYINT NOT NULL", result);
        }

        // small int

        [TestMethod]
        public void TestGetColumn_WithSmallInt()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.SmallInt());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] SMALLINT NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithSmallIntUnisgned_IgnoreUnsigned()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.SmallInt(true));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] SMALLINT NOT NULL", result);
        }

        // int

        [TestMethod]
        public void TestGetColumn_WithInt()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Int());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] INT NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithUnsignedInt_IgnoreUnsigned()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Int(true));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] INT NOT NULL", result);
        }

        // big int

        [TestMethod]
        public void TestGetColumn_WithBigInt()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.BigInt());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] BIGINT NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithBigIntUnisgned_IgnoreUnsigned()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.BigInt(true));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] BIGINT NOT NULL", result);
        }

        // bit

        [TestMethod]
        public void TestGetColumn_WithBit()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Bit());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] BIT NOT NULL", result);
        }

        // float

        [TestMethod]
        public void TestGetColumn_WithFloat()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Float());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] FLOAT NOT NULL", result);
        }


        [TestMethod]
        public void TestGetColumn_WithFloatAndDigits()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Float(2));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] DECIMAL(18,2) NOT NULL", result);
        }


        // char

        [TestMethod]
        public void TestGetColumn_WithChar()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1",ColumnType.Char());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NCHAR(10) NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithCharAndValue()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Char(100));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NCHAR(100) NOT NULL", result);
        }

        // varchar

        [TestMethod]
        public void TestGetColumn_WithString()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1");

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(255) NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithStringAndValue()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.VarChar(100));

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(100) NOT NULL", result);
        }

        // text

        [TestMethod]
        public void TestGetColumn_WithText()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Text());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NVARCHAR(MAX) NOT NULL", result);
        }

        // long text

        [TestMethod]
        public void TestGetColumn_WithLongText()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.LongText());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] NTEXT NOT NULL", result);
        }

        // datetime

        [TestMethod]
        public void TestGetColumn_WithDatetime()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.DateTime());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] DATETIME NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithDate()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Date());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] DATE NOT NULL", result);
        }

        [TestMethod]
        public void TestGetColumn_WithTime()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Time());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] TIME NOT NULL", result);
        }


        [TestMethod]
        public void TestGetColumn_WithTimestamp()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Timestamp());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] TIMESTAMP NOT NULL", result);
        }

        // blob


        [TestMethod]
        public void TestGetColumn_WithBlob()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.Blob());

            var result = service.GetColumn(table.GetColumn("column1"));

            Assert.AreEqual("[column1] VARBINARY(MAX) NOT NULL", result);
        }

        // drop table

        [TestMethod]
        public void TestDropTable()
        {
            var service = this.GetService();

            var result = service.GetDropTable("table1");
            var expected = "DROP TABLE IF EXISTS [dbo].[table1];\r";

            Assert.AreEqual(expected, result);
        }

        // create table

        [TestMethod]
        public void TestCreateTable_WithNoKeys()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddColumn("column1")
                .AddColumn("column2");

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[column2] NVARCHAR(255) NOT NULL\r);\r";
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestCreateTable_WithPrimaryKey()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1")
                .AddColumn("column1")
                .AddColumn("column2");

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[pk1] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[column2] NVARCHAR(255) NOT NULL\r);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCreateTable_WithPrimaryKeys()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1", ColumnType.Int())
                .AddPrimaryKey("pk2", ColumnType.Int())
                .AddColumn("column1");

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[pk1] INT NOT NULL,\r\t[pk2] INT NOT NULL,\r\t[column1] NVARCHAR(255) NOT NULL\r);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCreateTable_WithForeignKey()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1")
                .AddColumn("column1")
                .AddForeignKey("fk1", "table2", "id");

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[pk1] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[fk1] INT NOT NULL\r);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCreateTable_WithForeignKeys()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1")
                .AddColumn("column1")
                .AddForeignKey("fk1", "table2", "id")
                .AddForeignKey("fk2", "table2", "id2");

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[pk1] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[fk1] INT NOT NULL,\r\t[fk2] INT NOT NULL\r);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestCreateTable_WithTimestamps()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1")
                .AddColumn("column1")
                .AddTimestamps();

            var result = service.GetCreateTable(table);

            var expected = "CREATE TABLE [dbo].[table1] (\r\t[pk1] INT NOT NULL IDENTITY(1,1),\r\t[column1] NVARCHAR(255) NOT NULL,\r\t[created_at] DATETIME NULL DEFAULT CURRENT_TIMESTAMP,\r\t[updated_at] DATETIME NULL\r);\r";
            Assert.AreEqual(expected, result);
        }

        // alter table primary key

        [TestMethod]
        public void TestAltertableAddPrimaryKey()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                 .AddPrimaryKey("pk1")
                 .AddColumn("column1");

            var result = service.GetAddPrimaryKeyConstraint(table);

            var expected = "ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([pk1]);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAltertableAddPrimaryKey_WithPrimaryKeys()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                .AddPrimaryKey("pk1", ColumnType.Int())
                .AddPrimaryKey("pk2", ColumnType.Int())
                .AddColumn("column1");

            var result = service.GetAddPrimaryKeyConstraint(table);

            var expected = "ALTER TABLE [dbo].[table1] ADD PRIMARY KEY ([pk1],[pk2]);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAltertableAddPrimaryKey_WithNoPrimaryKey_ReturnsEmptyString()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("table1")
                 .AddColumn("column1");

            var result = service.GetAddPrimaryKeyConstraint(table);

            var expected = "";
            Assert.AreEqual(expected, result);
        }

        // alter table foreign key

        [TestMethod]
        public void TestAltertableAddPForeignKey()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("posts")
                .AddForeignKey("user_id", "users", "id");

            var result = service.GetAddForeignKeyConstraints(table);

            var expected = "ALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAltertableAddForeignKey_WithForeignKeys()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("posts")
                .AddForeignKey("user_id", "users", "id")
                .AddForeignKey("category_id", "categories", "id");

            var result = service.GetAddForeignKeyConstraints(table);

            var expected = "ALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([user_id]) REFERENCES [dbo].[users]([id]);\rALTER TABLE [dbo].[posts] ADD FOREIGN KEY ([category_id]) REFERENCES [dbo].[categories]([id]);\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestAltertableAddForeignKey_WithNoForeignKey_ReturnsEmptyString()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("posts");

            var result = service.GetAddForeignKeyConstraints(table);

            var expected = "";
            Assert.AreEqual(expected, result);
        }

        // seed

        [TestMethod]
        public void TestSeed_WithIdentity()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("users")
                .AddPrimaryKey("id")
                .AddColumn("username")
                .AddColumn("age")
                .Insert(SeedData.New.Set("id", 1).Set("username", "user 1").Set("age", 20))
                .Insert(SeedData.New.Set("id", 2).Set("username", "user 2").Set("age", 30));

            var result = service.GetSeeds(table);

            var expected = "SET IDENTITY_INSERT [dbo].[users] ON;\rINSERT INTO [dbo].[users] ([id],[username],[age]) VALUES (1,'user 1',20);\rINSERT INTO [dbo].[users] ([id],[username],[age]) VALUES (2,'user 2',30);\rSET IDENTITY_INSERT [dbo].[users] OFF;\r";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGetData_WithNoIdentity_DontSetIdentityOn()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("users")
                .AddPrimaryKey("id", ColumnType.Int())
                .AddColumn("username")
                .AddColumn("age")
                .Insert(SeedData.New.Set("id", 1).Set("username", "user 1").Set("age", 20))
                .Insert(SeedData.New.Set("id", 2).Set("username", "user 2").Set("age", 30));

            var result = service.GetSeeds(table);

            var expected = "INSERT INTO [dbo].[users] ([id],[username],[age]) VALUES (1,'user 1',20);\rINSERT INTO [dbo].[users] ([id],[username],[age]) VALUES (2,'user 2',30);\r";
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestSeed_WithNull()
        {
            var service = this.GetService();

            var table = new CreateTableCommand("users")
                .AddPrimaryKey("id", ColumnType.Int())
                .AddColumn("username")
                .AddColumn("age", true)
                .Insert(SeedData.New.Set("id", 1).Set("username", null).Set("age", null));

            var result = service.GetSeeds(table);

            var expected = "INSERT INTO [dbo].[users] ([id],[username],[age]) VALUES (1,NULL,NULL);\r";
            Assert.AreEqual(expected, result);
        }

        // get seed row

        [TestMethod]
        public void TestGetSeedRow()
        {
            var service = this.GetService();

            var result = service.GetSeedRow("table1", new Dictionary<string, object> { { "id",1 },{ "column1", "value 1" } });

            var expected = "INSERT INTO [dbo].[table1] ([id],[column1]) VALUES (1,'value 1');\r";
            Assert.AreEqual(expected, result);
        }


    }
}
