using System.Collections.Generic;

namespace EasyMigLib.Schema
{
    public class DatabaseSchema
    {
        internal List<string> databasesToDrop;
        internal List<string> databasesToCreate;
        internal List<string> databasesToCreateAndUse;

        internal Dictionary<string, CreateTableSchema> tablesToCreate;
        internal Dictionary<string, AlterTableSchema> tablesToAlter;
        internal List<string> tablesToDrop;

        internal Dictionary<string, StoredProcedureSchema> storedProceduresToCreate;
        internal List<string> storedProceduresToDrop;

        internal Dictionary<string, SeedTableSchema> tablesToSeed;

        public bool HasDatabasesToDrop => this.databasesToDrop.Count > 0;
        public bool HasDatabasesToCreate => this.databasesToCreate.Count > 0;
        public bool HasDatabasesToCreateAndUse => this.databasesToCreateAndUse.Count > 0;

        public bool HasTablesToCreate => this.tablesToCreate.Count > 0;
        public bool HasTablesToAlter => this.tablesToAlter.Count > 0;
        public bool HasTablesToDrop => this.tablesToDrop.Count > 0;

        public bool HasStoredProceduresToCreate => this.storedProceduresToCreate.Count > 0;
        public bool HasStoredProceduresToDrop => this.storedProceduresToDrop.Count > 0;

        public bool HasTablesToSeed => this.tablesToSeed.Count > 0;

        public int DatabasesToDropCount => this.databasesToDrop.Count;
        public int DatabasesToCreateCount => this.databasesToCreate.Count;
        public int DatabasesToCreateAndUseCount => this.databasesToCreateAndUse.Count;

        public int TablesToCreateCount => this.tablesToCreate.Count;
        public int TablesToAlterCount => this.tablesToAlter.Count;
        public int TablesToDropCount => this.tablesToDrop.Count;

        public int StoredProceduresToCreateCount => this.storedProceduresToCreate.Count;
        public int StoredProceduresToDropCount => this.storedProceduresToDrop.Count;

        public int TablesToSeedCount => this.tablesToSeed.Count;

        public DatabaseSchema()
        {
            this.databasesToDrop = new List<string>();
            this.databasesToCreate = new List<string>();
            this.databasesToCreateAndUse = new List<string>();

            this.tablesToCreate = new Dictionary<string, CreateTableSchema>();
            this.tablesToAlter = new Dictionary<string, AlterTableSchema>();
            this.tablesToDrop = new List<string>();

            this.storedProceduresToCreate = new Dictionary<string, StoredProcedureSchema>();
            this.storedProceduresToDrop = new List<string>();

            this.tablesToSeed = new Dictionary<string, SeedTableSchema>();
        }

        // drop database

        public bool HasDatabaseToDrop(string databaseName)
        {
            return this.databasesToDrop.Contains(databaseName);
        }

        public void DropDatabase(string databaseName)
        {
            if (!this.HasDatabaseToDrop(databaseName))
            {
                this.databasesToDrop.Add(databaseName);
            }
        }

        // create database

        public bool HasDatabaseToCreate(string databaseName)
        {
            return this.databasesToCreate.Contains(databaseName);
        }

        public void CreateDatabase(string databaseName)
        {
            if (this.HasDatabaseToCreate(databaseName)) { throw new EasyMigException("Database to create " + databaseName + " already registered"); }

            this.databasesToCreate.Add(databaseName);
        }

        // create and use database

        public bool HasDatabaseToCreateAndUse(string databaseName)
        {
            return this.databasesToCreateAndUse.Contains(databaseName);
        }

        public void CreateAndUseDatabase(string databaseName)
        {
            if (this.HasDatabaseToCreateAndUse(databaseName)) { throw new EasyMigException("Database to create and use " + databaseName + " already registered"); }

            this.databasesToCreateAndUse.Add(databaseName);
        }

        // create table

        public bool HasTableToCreate(string tableName)
        {
            return this.tablesToCreate.ContainsKey(tableName);
        }

        public CreateTableSchema CreateTable(string tableName)
        {
            if (this.HasTableToCreate(tableName)) { throw new EasyMigException("Table to create " + tableName + " already registered"); }

            var result = new CreateTableSchema(tableName);
            this.tablesToCreate[tableName] = result;
            return result;
        }

        // alter table

        public bool HasTableToAlter(string tableName)
        {
            return this.tablesToAlter.ContainsKey(tableName);
        }

        public AlterTableSchema AlterTable(string tableName)
        {
            if (this.HasTableToAlter(tableName))
            {
                return this.tablesToAlter[tableName];
            }
            else
            {
                var result = new AlterTableSchema(tableName);
                this.tablesToAlter[tableName] = result;
                return result;
            }
        }

        // drop table

        public bool HasTableToDrop(string tableName)
        {
            return this.tablesToDrop.Contains(tableName);
        }

        public void DropTable(string tableName)
        {
            if (!this.HasTableToDrop(tableName))
            {
                this.tablesToDrop.Add(tableName);
            }
        }

        // stored procedure

        public bool HasStoredProcedureToCreate(string procedureName)
        {
            return this.storedProceduresToCreate.ContainsKey(procedureName);
        }

        public StoredProcedureSchema GetStoredProcedureToCreate(string procedureName)
        {
            if (!this.HasStoredProcedureToCreate(procedureName)) { throw new EasyMigException("No Stored procedure to create " + procedureName + " registered"); }

            return this.storedProceduresToCreate[procedureName];
        }

        public StoredProcedureSchema CreateStoredProcedure(string procedureName)
        {
            if (this.HasStoredProcedureToCreate(procedureName)) { throw new EasyMigException("Stored procedure to create " + procedureName + " already registered"); }

            var result = new StoredProcedureSchema(procedureName);
            this.storedProceduresToCreate[procedureName] = result;
            return result;
        }

        // drop procedure

        public bool HasStoredProcedureToDrop(string procedureName)
        {
            return this.storedProceduresToDrop.Contains(procedureName);
        }

        public void DropStoredProcedure(string procedureName)
        {
            if (!this.HasStoredProcedureToDrop(procedureName))
            {
                this.storedProceduresToDrop.Add(procedureName);
            }
        }

        // seed

        public bool HasSeedTable(string tableName)
        {
            return this.tablesToSeed.ContainsKey(tableName);
        }

        public SeedTableSchema GetSeedTable(string tableName)
        {
            if (!this.HasSeedTable(tableName)) { throw new EasyMigException("No Seed Table " + tableName + " registered"); }

            return this.tablesToSeed[tableName];
        }

        public SeedTableSchema SeedTable(string tableName)
        {
            if (this.HasSeedTable(tableName))
            {
                return this.GetSeedTable(tableName);
            }
            else
            {
                var result = new SeedTableSchema(tableName);
                this.tablesToSeed[tableName] = result;
                return result;
            }
        }

        // clear

        public void ClearSeeders()
        {
            this.tablesToSeed.Clear();
        }

        public void ClearMigrations()
        {
            this.databasesToDrop.Clear();
            this.databasesToCreate.Clear();
            this.databasesToCreateAndUse.Clear();

            this.tablesToCreate.Clear();
            this.tablesToAlter.Clear();
            this.tablesToDrop.Clear();

            this.storedProceduresToCreate.Clear();
            this.storedProceduresToDrop.Clear();
        }
    }
}
