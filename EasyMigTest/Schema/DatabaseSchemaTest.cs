using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyMigLib.Schema;
using System.Collections.Generic;

namespace EasyMigTest.Schema
{
    [TestClass]
    public class DatabaseSchemaTest
    {

        public DatabaseSchema GetService()
        {
            return new DatabaseSchema();
        }

        // databases to drop

        [TestMethod]
        public void TestDatabasesToDrop()
        {
            var dbName = "db1";

            var service = this.GetService();

            service.DropDatabase(dbName);

            Assert.IsTrue(service.HasDatabaseToDrop(dbName));
        }

        [TestMethod]
        public void TestDatabasesToDrop_IsNotAddedIfAlreadyregistered()
        {
            var dbName = "db1";

            var service = this.GetService();

            service.DropDatabase(dbName);

            Assert.IsTrue(service.HasDatabaseToDrop(dbName));

            service.DropDatabase(dbName);

            Assert.AreEqual(1, service.DatabasesToDropCount);
        }

        // databases to create

        [TestMethod]
        public void TestDatabasesToCreate()
        {
            var dbName = "db1";

            var service = this.GetService();

            service.CreateDatabase(dbName);

            Assert.IsTrue(service.HasDatabaseToCreate(dbName));
        }

        [TestMethod]
        public void TestDatabasesToCreate_ThrowExceptionIfNameAlreaydregistered()
        {
            bool failed = false;

            var dbName = "db1";

            var service = this.GetService();

            service.CreateDatabase(dbName);

            try
            {
                service.CreateDatabase(dbName);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // databases to create and use

        [TestMethod]
        public void TestDatabasesToCreateAndUse()
        {
            var dbName = "db1";

            var service = this.GetService();

            service.CreateAndUseDatabase(dbName);

            Assert.IsTrue(service.HasDatabaseToCreateAndUse(dbName));
        }

        [TestMethod]
        public void TestDatabasesToCreateAndUse_ThrowExceptionIfNameAlreaydregistered()
        {
            bool failed = false;

            var dbName = "db1";

            var service = this.GetService();

            service.CreateAndUseDatabase(dbName);

            try
            {
                service.CreateAndUseDatabase(dbName);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // create table

        [TestMethod]
        public void TestCreateTable()
        {
            var tableName = "table1";

            var service = this.GetService();

            service.CreateTable(tableName);

            Assert.IsTrue(service.HasTableToCreate(tableName));
        }

        [TestMethod]
        public void TestCreateTable_ThrowExceptionIfNameAlreaydregistered()
        {
            bool failed = false;

            var tableName = "table1";

            var service = this.GetService();

            service.CreateTable(tableName);

            try
            {
                service.CreateTable(tableName);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // add column

        [TestMethod]
        public void TestCreateTable_AddColumn()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddColumn(columnName);

            Assert.IsTrue(table.HasColumn(columnName));
        }

        [TestMethod]
        public void TestCreateTable_AddColumn_Failed_WithSameName()
        {
            bool failed = false;
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            try
            {
                var table = service.CreateTable(tableName)
                              .AddColumn(columnName)
                              .AddColumn(columnName);
            }
            catch (Exception)
            {
                failed = true;
            }
          
            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateTable_AddColumnWithDefaults()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddColumn(columnName);

            Assert.IsTrue(table.HasColumn(columnName));

            var column = table.GetColumn(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestCreateTable_AddColumnWithNullable()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddColumn(columnName, true);

            Assert.IsTrue(table.HasColumn(columnName));

            var column = table.GetColumn(columnName);

            Assert.AreEqual(true, column.Nullable);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestCreateTable_AddColumnWithValues()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddColumn(columnName,ColumnType.Int(),true,10,true);

            Assert.IsTrue(table.HasColumn(columnName));

            var column = table.GetColumn(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(true, column.Nullable);
            Assert.AreEqual(10, column.DefaultValue);
            Assert.AreEqual(true, column.Unique);
        }

        // add primary key

        [TestMethod]
        public void TestCreateTable_AddPrimaryKey()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddPrimaryKey(columnName);

            Assert.IsTrue(table.HasPrimaryKey(columnName));
        }

        [TestMethod]
        public void TestCreateTable_AddprimaryKey_WithSameName_Fail()
        {
            bool failed = false;
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            try
            {
                var table = service.CreateTable(tableName)
                              .AddPrimaryKey(columnName)
                              .AddPrimaryKey(columnName);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateTable_AddPrimaryKey_WithDefaults()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddPrimaryKey(columnName);

            var column = table.GetPrimaryKey(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(true, column.AutoIncrement);
            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestCreateTable_AddPrimaryKey_WithValues()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddPrimaryKey(columnName, ColumnType.VarChar());

            var column = table.GetPrimaryKey(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual(false, column.AutoIncrement);
            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestCreateTable_AddPrimaryKey_WithVarCharAndAutoIncrement_Fail()
        {
            bool failed = false;

            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            try
            {
                var table = service.CreateTable(tableName)
                    .AddPrimaryKey(columnName, ColumnType.VarChar(), true);
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // add foreign key

        [TestMethod]
        public void TestCreateTable_AddForeignKey()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddForeignKey(columnName, "table2", "id");

            Assert.IsTrue(table.HasForeignKey(columnName));
        }

        [TestMethod]
        public void TestCreateTable_AddForeignKey_WithSameName_Fail()
        {
            bool failed = false;

            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            try
            {
                var table = service.CreateTable(tableName)
                    .AddForeignKey(columnName, "table2", "id")
                    .AddForeignKey(columnName, "table3", "id");

                Assert.IsTrue(table.HasForeignKey(columnName));
            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        [TestMethod]
        public void TestCreateTable_AddForeignKey_WithDefaults()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddForeignKey(columnName, "table2", "id");

            var column = table.GetForeignKey(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual("table2", column.TableReferenced);
            Assert.AreEqual("id", column.PrimaryKeyReferenced);

            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestCreateTable_AddForeignKey_WithValues()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddForeignKey(columnName, ColumnType.VarChar(), "table2", "id", true, "default value");

            var column = table.GetForeignKey(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual("table2", column.TableReferenced);
            Assert.AreEqual("id", column.PrimaryKeyReferenced);

            Assert.AreEqual(true, column.Nullable);
            Assert.AreEqual("default value", column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        // all columns

        [TestMethod]
        public void TestCreateTable_CheckAllColumnType()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddPrimaryKey("id")
                .AddColumn(columnName)
                .AddForeignKey("table2_id", "table2", "id");


            Assert.IsTrue(table.HasColumn("id"));
            Assert.IsTrue(table.HasColumn(columnName));
            Assert.IsTrue(table.HasColumn("table2_id"));
        }


        [TestMethod]
        public void TestCreateTable_GetAllColumnType()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.CreateTable(tableName)
                .AddPrimaryKey("id")
                .AddColumn(columnName)
                .AddForeignKey("table2_id", "table2", "id");

            var column = table.GetColumn("id");

            Assert.AreEqual(typeof(PrimaryKeyColumn), column.GetType());
            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual("id", column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);

            var column2 = table.GetColumn(columnName);

            Assert.AreEqual(tableName, column2.TableName);
            Assert.AreEqual(columnName, column2.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column2.ColumnType.GetType());
            Assert.AreEqual(false, column2.Nullable);
            Assert.AreEqual(null, column2.DefaultValue);
            Assert.AreEqual(false, column2.Unique);

            var column3 = table.GetColumn("table2_id");

            Assert.AreEqual(typeof(ForeignKeyColumn), column3.GetType());
            Assert.AreEqual(tableName, column3.TableName);
            Assert.AreEqual("table2_id", column3.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column3.ColumnType.GetType());
            Assert.AreEqual("table2", ((ForeignKeyColumn)column3).TableReferenced);
            Assert.AreEqual("id", ((ForeignKeyColumn)column3).PrimaryKeyReferenced);
            Assert.AreEqual(false, column3.Nullable);
            Assert.AreEqual(null, column3.DefaultValue);
            Assert.AreEqual(false, column3.Unique);
        }

        // seed on init

        [TestMethod]
        public void TestSeedOnInit()
        {
            var service = this.GetService();
            var tableName = "table1";

            var table = service.CreateTable(tableName)
                 .Insert(SeedData.New.Set("column1", "value1"))
                 .Insert(SeedData.New.Set("column1", "value2"));

            Assert.AreEqual(2, table.SeedRowCount);
            Assert.AreEqual("value1", table.GetSeedRow(0)["column1"]);
            Assert.AreEqual("value2", table.GetSeedRow(1)["column1"]);
        }

        [TestMethod]
        public void TestSeedOnInit_WithDictionary()
        {
            var service = this.GetService();
            var tableName = "table1";

            var table = service.CreateTable(tableName)
                 .Insert(new Dictionary<string, object> { { "column1", "value1" } })
                 .Insert(new Dictionary<string, object> { { "column1", "value2" } });

            Assert.AreEqual(2, table.SeedRowCount);
            Assert.AreEqual("value1", table.GetSeedRow(0)["column1"]);
            Assert.AreEqual("value2", table.GetSeedRow(1)["column1"]);
        }


        // Alter table

        [TestMethod]
        public void TestAlterTable()
        {
            var tableName = "table1";

            var service = this.GetService();

            var table = service.AlterTable(tableName);

            Assert.IsTrue(service.HasTableToAlter(tableName));
        }

        [TestMethod]
        public void TestAlterTable_ReturnsSameTable()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName).AddColumn(columnName);

            var result = service.AlterTable(tableName);

            Assert.IsTrue(result.HasColumnToAdd(columnName));
        }

        // alter add column 

        [TestMethod]
        public void TestAlterTable_AddColumn()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddColumn(columnName);

            Assert.IsTrue(table.HasColumnToAdd(columnName));
        }

        [TestMethod]
        public void TestAlterTable_AddColumn_WithDefaults()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddColumn(columnName);

            var column = table.GetColumnToAdd(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual(false, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestAlterTable_AddColumn_WithNullable()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddColumn(columnName, true);

            var column = table.GetColumnToAdd(columnName);

            Assert.AreEqual(true, column.Nullable);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestAlterTable_AddColumn_WithValues()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddColumn(columnName, ColumnType.Int(),true,10,true);

            var column = table.GetColumnToAdd(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(true, column.Nullable);
            Assert.AreEqual(10, column.DefaultValue);
            Assert.AreEqual(true, column.Unique);
        }

        // alter modify column 

        [TestMethod]
        public void TestAlterTable_ModifyColumn()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName).ModifyColumn(columnName, ColumnType.Int(), true);

            Assert.IsTrue(table.HasColumnToModify(columnName));

            var column = table.GetColumnToModify(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(true, column.Nullable);
            Assert.AreEqual(null, column.DefaultValue);
            Assert.AreEqual(false, column.Unique);
        }

        [TestMethod]
        public void TestAlterTable_ModifyColumnLong()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName).ModifyColumn(columnName, ColumnType.Int(), true, 10, true);

            Assert.IsTrue(table.HasColumnToModify(columnName));

            var column = table.GetColumnToModify(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
            Assert.AreEqual(true, column.Nullable);
            Assert.AreEqual(10, column.DefaultValue);
            Assert.AreEqual(true, column.Unique);
        }

        // drop column

        [TestMethod]
        public void TestAlterTable_DropColumn()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName).DropColumn(columnName);

            Assert.IsTrue(table.HasColumnToDrop(columnName));
        }

        [TestMethod]
        public void TestAlterTable_DropColumn_IsNotAddedIfRegistered()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .DropColumn(columnName);

            Assert.IsTrue(table.HasColumnToDrop(columnName));

            table.DropColumn(columnName);

            Assert.AreEqual(1, table.ColumnsToDropCount);
        }

        // alter add primary key

        [TestMethod]
        public void TestAlterTable_AddPrimaryKeyConstraint()
        {
            var tableName = "table1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddPrimaryKeyConstraint(new string[] { "p1", "p2" });

            Assert.IsTrue(table.HasPrimaryKeyConstraint);

            var pk = table.GetPrimaryKeyConstraint();

            Assert.AreEqual(tableName, pk.TableName);
            Assert.AreEqual("p1", pk.PrimaryKeys[0]);
            Assert.AreEqual("p2", pk.PrimaryKeys[1]);
        }

        [TestMethod]
        public void TestAlterTable_AddPrimaryKeyConstraint_Fail()
        {
            bool failed = false;

            var tableName = "table1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
               .AddPrimaryKeyConstraint(new string[] { "p1", "p2" });

            try
            {
                table.AddPrimaryKeyConstraint(new string[] { "p1", "p2" });

            }
            catch (Exception)
            {
                failed = true;
            }

            Assert.IsTrue(failed);
        }

        // alter add foreign key

        [TestMethod]
        public void TestAlterTable_AddForeignKeyConstraint()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddForeignKeyConstraint(columnName, "table2","id");

            Assert.IsTrue(table.HasForeignKeyConstraint(columnName));
        }

        [TestMethod]
        public void TestAlterTable_AddForeignKeyConstraint_WithDefaults()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddForeignKeyConstraint(columnName, "table2", "id");

            var column = table.GetForeignKeyConstraint(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(IntColumnType), column.ColumnType.GetType());
        }

        [TestMethod]
        public void TestAlterTable_AddForeignKeyConstraint_WithColumnType()
        {
            var tableName = "table1";
            var columnName = "column1";

            var service = this.GetService();

            var table = service.AlterTable(tableName)
                .AddForeignKeyConstraint(columnName, ColumnType.VarChar(), "table2", "id");

            var column = table.GetForeignKeyConstraint(columnName);

            Assert.AreEqual(tableName, column.TableName);
            Assert.AreEqual(columnName, column.ColumnName);
            Assert.AreEqual(typeof(VarCharColumnType), column.ColumnType.GetType());
        }

        // stored procedure

        [TestMethod]
        public void TestCreateStoredProcedure()
        {
            var service = this.GetService();
            var procedureName = "p1";

            var result = service.CreateStoredProcedure(procedureName);

            Assert.IsTrue(service.HasStoredProcedureToCreate(procedureName));
        }

        [TestMethod]
        public void TestCreateStoredProcedure_WithParams()
        {
            var service = this.GetService();
            var procedureName = "p1";

            var result = service.CreateStoredProcedure(procedureName)
                .AddInParameter("p_id",ColumnType.Int(),10)
                .AddOutParameter("p_result",ColumnType.VarChar())
                .SetBody("select * from users");

            Assert.AreEqual(procedureName, result.ProcedureName);
            Assert.AreEqual("select * from users", result.Body);

            Assert.AreEqual(2, result.Parameters.Count);

            var p_id = result.Parameters["p_id"];
            Assert.AreEqual("p_id", p_id.ParameterName);
            Assert.AreEqual(StoredProcedureParameterDirection.IN, p_id.Direction);
            Assert.AreEqual(typeof(IntColumnType), p_id.ParameterType.GetType());
            Assert.AreEqual(10, p_id.DefaultValue);

            var p_result = result.Parameters["p_result"];
            Assert.AreEqual("p_result", p_result.ParameterName);
            Assert.AreEqual(StoredProcedureParameterDirection.OUT, p_result.Direction);
            Assert.AreEqual(typeof(VarCharColumnType), p_result.ParameterType.GetType());
            Assert.AreEqual(null, p_result.DefaultValue);
        }

        [TestMethod]
        public void TestDropStoredProcedure()
        {
            var service = this.GetService();
            var procedureName = "p1";

            service.DropStoredProcedure(procedureName);

            Assert.IsTrue(service.HasStoredProcedureToDrop(procedureName));
        }

        [TestMethod]
        public void TestDropStoredProcedure_DoNotAddSameProcedureName()
        {
            var service = this.GetService();
            var procedureName = "p1";

            service.DropStoredProcedure(procedureName);

            Assert.IsTrue(service.HasStoredProcedureToDrop(procedureName));

            service.DropStoredProcedure(procedureName);

            Assert.AreEqual(1, service.StoredProceduresToDropCount);
        }

        // seeders

        [TestMethod]
        public void TestSeedTable()
        {
            var service = this.GetService();
            var tableName = "table1";

           var table = service.SeedTable(tableName)
                .Insert(SeedData.New.Set("column1","value1"))
                .Insert(SeedData.New.Set("column1", "value2"));

            Assert.AreEqual(2, table.RowCount);
            Assert.AreEqual("value1", table.GetRow(0)["column1"]);
            Assert.AreEqual("value2", table.GetRow(1)["column1"]);
        }

        [TestMethod]
        public void TestSeedTable_ReturnsSameTable()
        {
            var service = this.GetService();
            var tableName = "table1";

            var table = service.SeedTable(tableName)
                 .Insert(SeedData.New.Set("column1", "value1"))
                 .Insert(SeedData.New.Set("column1", "value2"));

           var result =  service.SeedTable(tableName);

           Assert.AreEqual(2, result.RowCount);
        }

        [TestMethod]
        public void TestClearSeeders()
        {
            var service = this.GetService();

            service.SeedTable("table1").Insert(SeedData.New.Set("column1", "value1"));


            Assert.IsTrue(service.HasTablesToSeed);

            service.ClearSeeders();

            Assert.IsFalse(service.HasTablesToSeed);
        }

        [TestMethod]
        public void TestClearMigrations()
        {
            var service = this.GetService();

            service.DropDatabase("db1");
            service.CreateDatabase("db1");
            service.CreateAndUseDatabase("db1");

            service.CreateTable("table1");
            service.AlterTable("table1").AddColumn("column1");
            service.DropTable("table1");

            service.DropStoredProcedure("p1");
            service.CreateStoredProcedure("p1");

            Assert.IsTrue(service.HasDatabasesToCreate);
            Assert.IsTrue(service.HasDatabasesToCreateAndUse);
            Assert.IsTrue(service.HasDatabasesToDrop);
            Assert.IsTrue(service.HasTablesToCreate);
            Assert.IsTrue(service.HasTablesToAlter);
            Assert.IsTrue(service.HasTablesToDrop);
            Assert.IsTrue(service.HasStoredProceduresToCreate);
            Assert.IsTrue(service.HasStoredProceduresToDrop);

            service.ClearMigrations();

            Assert.IsFalse(service.HasDatabasesToCreate);
            Assert.IsFalse(service.HasDatabasesToCreateAndUse);
            Assert.IsFalse(service.HasDatabasesToDrop);
            Assert.IsFalse(service.HasTablesToCreate);
            Assert.IsFalse(service.HasTablesToAlter);
            Assert.IsFalse(service.HasTablesToDrop);
            Assert.IsFalse(service.HasStoredProceduresToCreate);
            Assert.IsFalse(service.HasStoredProceduresToDrop);
        }
    }
}
