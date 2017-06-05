using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Commands;
using EasyMigLib;
using EasyMigLib.Services;

namespace EasyMigLibTest.Commands
{
    [TestClass]
    public class CreateTableCommandTest
    {
        // check

        // same column name

        [TestMethod]
        public void TestAddColumn_WithSameName_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            table.AddColumn("column1");

            try
            {
                table.AddColumn("column1");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithNotSameName_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            table.AddColumn("column1");

            try
            {
                table.AddColumn("column2");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }


        // check default value types

        [TestMethod]
        public void TestAddColumn_WithStringAndInvalidValue_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.String(), false, 10);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithStringAndValidValue_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.String(), false, "my value");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithIntAndInvalidValue_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Int(), false, "invalid value");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithIntAndValidValue_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Int(), false, 10);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithFloatAndInvalidValue_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Float(), false, "invalid value");
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithFloatAndValidValue_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Float(), false, 10.99);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithFloatAndInt_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Float(), false, 10);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithBitAndInvalidValue_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Boolean(), false, 10);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddColumn_WithBitAndValidValue_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddColumn("column1", ColumnType.Boolean(), false, true);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }


        // check primary key auto increment

        [TestMethod]
        public void TestAddPrimaryColumn_WithInvalidAutoIncrement_Fail()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddPrimaryKey("id", ColumnType.String(), true);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestAddPrimaryColumn_WithValidAutoIncrement_Success()
        {
            bool failed = false;
            var table = new CreateTableCommand("table1");

            try
            {
                table.AddPrimaryKey("id", ColumnType.Int(), true);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsFalse(failed);
        }


        // add column

        [TestMethod]
        public void TestAddColumn_SimpleMethod()
        {
            var table = new CreateTableCommand("table1");
            table.AddColumn("column1");
            table.AddColumn("column2", true);

            Assert.IsTrue(table.HasColumn("column1"));
            Assert.IsTrue(table.HasColumn("column2"));

            var result = table.GetColumn("column1");

            Assert.AreEqual("column1", result.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result.ColumnType.GetType());
            Assert.AreEqual(false, result.Nullable);
            Assert.AreEqual(null, result.DefaultValue);
            Assert.AreEqual(false, result.Unique);

            var result2 = table.GetColumn("column2");

            Assert.AreEqual("column2", result2.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result2.ColumnType.GetType());
            Assert.AreEqual(true, result2.Nullable);
            Assert.AreEqual(null, result2.DefaultValue);
            Assert.AreEqual(false, result2.Unique);
        }

        [TestMethod]
        public void TestAddColumn()
        {
            var table = new CreateTableCommand("table1");
            table.AddColumn("column1", ColumnType.String(), true, "default value");
            table.AddColumn("column2", ColumnType.Boolean(), false, true);
            table.AddColumn("column3", ColumnType.String(100), false, "default value 2", true);

            Assert.IsTrue(table.HasColumn("column1"));
            Assert.IsTrue(table.HasColumn("column2"));
            Assert.IsTrue(table.HasColumn("column3"));

            var result = table.GetColumn("column1");

            Assert.AreEqual("column1", result.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result.ColumnType.GetType());
            Assert.AreEqual(true, result.Nullable);
            Assert.AreEqual("default value", result.DefaultValue);
            Assert.AreEqual(false, result.Unique);

            var result2 = table.GetColumn("column2");

            Assert.AreEqual("column2", result2.ColumnName);
            Assert.AreEqual(typeof(BooleanColumnType), result2.ColumnType.GetType());
            Assert.AreEqual(false, result2.Nullable);
            Assert.AreEqual(true, result2.DefaultValue);
            Assert.AreEqual(false, result2.Unique);

            var result3 = table.GetColumn("column3");

            Assert.AreEqual("column3", result3.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result3.ColumnType.GetType());
            Assert.AreEqual(100, ((StringColumnType)result3.ColumnType).Length);
            Assert.AreEqual(false, result3.Nullable);
            Assert.AreEqual("default value 2", result3.DefaultValue);
            Assert.AreEqual(true, result3.Unique);
        }


        // add primary column

        [TestMethod]
        public void TestAddPrimaryColumn()
        {
            var table = new CreateTableCommand("table1");
            table.AddPrimaryKey("id1");
            table.AddPrimaryKey("id2", ColumnType.String());

            Assert.IsTrue(table.HasPrimaryKeys);
            Assert.IsTrue(table.HasPrimaryKey("id1"));
            Assert.IsTrue(table.HasPrimaryKey("id2"));

            var result = table.GetPrimaryKey("id1");

            Assert.AreEqual("id1", result.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), result.ColumnType.GetType());
            Assert.AreEqual(false, result.Nullable);
            Assert.AreEqual(null, result.DefaultValue);
            Assert.AreEqual(true, result.AutoIncrement);

            var result2 = table.GetPrimaryKey("id2");

            Assert.AreEqual("id2", result2.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result2.ColumnType.GetType());
            Assert.AreEqual(false, result2.Nullable);
            Assert.AreEqual(null, result2.DefaultValue);
            Assert.AreEqual(false, result2.AutoIncrement);
        }

        [TestMethod]
        public void TestHasColumn_WithPrimaryKeys_ReturnTrue()
        {
            var table = new CreateTableCommand("table1");
            table.AddPrimaryKey("id1");
            table.AddPrimaryKey("id2", ColumnType.String());
            table.AddColumn("column1");

            Assert.IsTrue(table.HasColumn("id1"));
            Assert.IsTrue(table.HasColumn("id2"));
            Assert.IsTrue(table.HasColumn("column1"));
        }

        [TestMethod]
        public void TestGetColumn_WithPrimaryKeys_ReturnColumns()
        {
            var table = new CreateTableCommand("table1");
            table.AddPrimaryKey("id1");
            table.AddPrimaryKey("id2", ColumnType.String());
            table.AddColumn("column1");

            var result = table.GetColumn("id1");

            Assert.AreEqual("id1", result.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), result.ColumnType.GetType());
            Assert.AreEqual(false, result.Nullable);
            Assert.AreEqual(null, result.DefaultValue);
            Assert.AreEqual(true, ((PrimaryKeyColumn)result).AutoIncrement);

            var result2 = table.GetColumn("id2");

            Assert.AreEqual("id2", result2.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result2.ColumnType.GetType());
            Assert.AreEqual(false, result2.Nullable);
            Assert.AreEqual(null, result2.DefaultValue);
            Assert.AreEqual(false, ((PrimaryKeyColumn)result2).AutoIncrement);

            var result3 = table.GetColumn("column1");

            Assert.AreEqual("column1", result3.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result3.ColumnType.GetType());
            Assert.AreEqual(false, result3.Nullable);
            Assert.AreEqual(null, result3.DefaultValue);
        }

        // foreign key

        [TestMethod]
        public void TestAddForeignKeyColumn()
        {
            var table = new CreateTableCommand("table1");
            table.AddForeignKey("column1", "table2", "id1");
            table.AddForeignKey("column2", "table3", "id2", true);
            table.AddForeignKey("column3", ColumnType.String(), "table4", "id3");
            table.AddForeignKey("column4", ColumnType.String(), "table5", "id4", true, "default value");

            Assert.IsTrue(table.HasForeignKeys);
            Assert.IsTrue(table.HasForeignKey("column1"));
            Assert.IsTrue(table.HasForeignKey("column2"));
            Assert.IsTrue(table.HasForeignKey("column3"));
            Assert.IsTrue(table.HasForeignKey("column4"));

            var result = table.GetForeignKey("column1");

            Assert.AreEqual("column1", result.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), result.ColumnType.GetType());
            Assert.AreEqual(true, ((IntColumnType)result.ColumnType).Unsigned);
            Assert.AreEqual(false, result.Nullable);
            Assert.AreEqual(null, result.DefaultValue);
            Assert.AreEqual("table2", result.TableReferenced);
            Assert.AreEqual("id1", result.PrimaryKeyReferenced);

            var result2 = table.GetForeignKey("column2");

            Assert.AreEqual("column2", result2.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), result2.ColumnType.GetType());
            Assert.AreEqual(true, ((IntColumnType)result2.ColumnType).Unsigned);
            Assert.AreEqual(true, result2.Nullable);
            Assert.AreEqual(null, result2.DefaultValue);
            Assert.AreEqual("table3", result2.TableReferenced);
            Assert.AreEqual("id2", result2.PrimaryKeyReferenced);

            var result3 = table.GetForeignKey("column3");

            Assert.AreEqual("column3", result3.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result3.ColumnType.GetType());
            Assert.AreEqual(false, result3.Nullable);
            Assert.AreEqual(null, result3.DefaultValue);
            Assert.AreEqual("table4", result3.TableReferenced);
            Assert.AreEqual("id3", result3.PrimaryKeyReferenced);

            var result4 = table.GetForeignKey("column4");

            Assert.AreEqual("column4", result4.ColumnName);
            Assert.AreEqual(typeof(StringColumnType), result4.ColumnType.GetType());
            Assert.AreEqual(true, result4.Nullable);
            Assert.AreEqual("default value", result4.DefaultValue);
            Assert.AreEqual("table5", result4.TableReferenced);
            Assert.AreEqual("id4", result4.PrimaryKeyReferenced);
        }


        [TestMethod]
        public void TestAddForeignKeyColumn_WithGetColumn()
        {
            var table = new CreateTableCommand("table1");
            table.AddForeignKey("column1", "table2", "id1");
            table.AddForeignKey("column2", "table3", "id2", true);
            table.AddForeignKey("column3", ColumnType.String(), "table4", "id3");
            table.AddForeignKey("column4", ColumnType.String(), "table5", "id4", true, "default value");


            var result = table.GetColumn("column1");
            Assert.AreEqual(typeof(ForeignKeyColumn), result.GetType());

            var result2 = table.GetForeignKey("column2");
            Assert.AreEqual(typeof(ForeignKeyColumn), result2.GetType());

            var result3 = table.GetForeignKey("column3");
            Assert.AreEqual(typeof(ForeignKeyColumn), result3.GetType());

            var result4 = table.GetForeignKey("column4");
            Assert.AreEqual(typeof(ForeignKeyColumn), result4.GetType());
        }

        [TestMethod]
        public void TestHasColumn_WithKeysAndColumns_ReturnsTrue()
        {
            var table = new CreateTableCommand("table1");
            table.AddForeignKey("column1", "table2", "id1");
            table.AddForeignKey("column2", "table3", "id2", true);
            table.AddForeignKey("column3", ColumnType.String(), "table4", "id3");
            table.AddForeignKey("column4", ColumnType.String(), "table5", "id4", true, "default value");
            table.AddColumn("column5");
            table.AddPrimaryKey("column6");

            Assert.IsTrue(table.HasColumn("column1"));
            Assert.IsTrue(table.HasColumn("column2"));
            Assert.IsTrue(table.HasColumn("column3"));
            Assert.IsTrue(table.HasColumn("column4"));
            Assert.IsTrue(table.HasColumn("column5"));
            Assert.IsTrue(table.HasColumn("column6"));
        }

        [TestMethod]
        public void TestGetColumn_WithKeysAndColumns_ReturnsColumns()
        {
            var table = new CreateTableCommand("table1");
            table.AddForeignKey("column1", "table2", "id1");
            table.AddForeignKey("column2", "table3", "id2", true);
            table.AddForeignKey("column3", ColumnType.String(), "table4", "id3");
            table.AddForeignKey("column4", ColumnType.String(), "table5", "id4", true, "default value");
            table.AddColumn("column5");
            table.AddPrimaryKey("column6");

            Assert.AreEqual(typeof(ForeignKeyColumn), table.GetColumn("column1").GetType());
            Assert.AreEqual(typeof(ForeignKeyColumn), table.GetColumn("column2").GetType());
            Assert.AreEqual(typeof(ForeignKeyColumn), table.GetColumn("column3").GetType());
            Assert.AreEqual(typeof(ForeignKeyColumn), table.GetColumn("column4").GetType());
            Assert.AreEqual(typeof(MigrationColumn), table.GetColumn("column5").GetType());
            Assert.AreEqual(typeof(PrimaryKeyColumn), table.GetColumn("column6").GetType());
        }

        // timestamps

        [TestMethod]
        public void TestAddTimestamps()
        {
            var table = new CreateTableCommand("table1");

            Assert.IsFalse(table.Timestamps);

            table.AddTimestamps();

            Assert.IsTrue(table.Timestamps);
        }


        // get query

        [TestMethod]
        public void TestGetQuery_WithSql()
        {
            var command = new CreateTableCommand("posts");
            command.AddPrimaryKey("id")
                .AddColumn("title")
                .AddColumn("content")
                .AddForeignKey("user_id", "users", "id");

            var result = command.GetQuery(new SqlQueryService());

            Assert.AreEqual("CREATE TABLE [dbo].[posts] (\r\t[id] INT NOT NULL IDENTITY(1,1),\r\t[title] NVARCHAR(255) NOT NULL,\r\t[content] NVARCHAR(255) NOT NULL,\r\t[user_id] INT NOT NULL\r);\r", result);
        }

        [TestMethod]
        public void TestGetQuery_WithMySQL()
        {
            var command = new CreateTableCommand("posts");
            command.AddPrimaryKey("id")
                .AddColumn("title")
                .AddColumn("content")
                .AddForeignKey("user_id", "users", "id");

            var result = command.GetQuery(new MySQLQueryService());

            Assert.AreEqual("CREATE TABLE `posts` (\r\t`id` INT UNSIGNED NOT NULL,\r\t`title` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL,\r\t`content` VARCHAR(255) COLLATE utf8mb4_unicode_ci NOT NULL,\r\t`user_id` INT UNSIGNED NOT NULL\r) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;\r", result);
        }

    }
}
