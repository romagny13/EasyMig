using EasyMigLib;
using EasyMigLib.MigrationReflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyMigApp.Models
{
 
    public class MigrationService : IMigrationService
    {
        protected IMigrationAssemblyService assemblyService;

        public MigrationService()
            :this(new MigrationAssemblyService())
        { }

        public MigrationService(IMigrationAssemblyService assemblyService)
        {
            this.assemblyService = assemblyService;
        }

        public Assembly LoadFromFile(string path)
        {
            return Assembly.LoadFile(path);
        }

        public string GetAssemblyDirectory(string assemblyPath)
        {
            var assembly = this.LoadFromFile(assemblyPath);
            string codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }

        public List<Type> GetMigrations(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Migration)) && !t.IsAbstract).ToList();
        }

        public List<Type> GetSeeders(Assembly assembly)
        {
            return assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Seeder)) && !t.IsAbstract).ToList();
        }

        public List<RecognizedMigrationFile> ResolveMigrationTypes(string assemblyPath)
        {
           return this.assemblyService.FindTypes<Migration>(assemblyPath);
        }

        public List<RecognizedMigrationFile> ResolveSeederTypes(string assemblyPath)
        {
            return this.assemblyService.FindTypes<Seeder>(assemblyPath);
        }

        public bool IsAttachDbFilename(string connectionString)
        {
            var regex = new Regex("AttachDbFilename=", RegexOptions.IgnoreCase);
            return regex.IsMatch(connectionString);
        }

        public bool IsSqlClient(string providerName)
        {
           return providerName == "System.Data.SqlClient";
        }

        public void CreateMigrationScript(string assemblyPath, string providerName, string fileName, string procedureFileName, string engine)
        {

            if (this.IsSqlClient(providerName))
            {
                EasyMig.ToSqlServer.CreateMigrationScript(assemblyPath, fileName);
                EasyMig.ToSqlServer.CreateStoredProcedureScript(assemblyPath, procedureFileName);
            }
            else
            {
                EasyMig.ToMySql.CreateMigrationScript(assemblyPath, fileName, engine);
                EasyMig.ToMySql.CreateStoredProcedureScript(assemblyPath, procedureFileName);
            }
        }

        public void CreateSeedScript(string assemblyPath, string providerName, string fileName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                EasyMig.ToSqlServer.CreateSeedScript(assemblyPath, fileName);
            }
            else
            {

                EasyMig.ToMySql.CreateSeedScript(assemblyPath,  fileName, engine);
            }
        }

        public void UpAll(string assemblyPath, string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoMigrationsForAssembly(assemblyPath, connectionString);
                }
                else
                {
                    EasyMig.ToSqlServer.DoMigrationsForAssembly(assemblyPath, connectionString);
                }
            }
            else
            {
                EasyMig.ToMySql.DoMigrationsForAssembly(assemblyPath, connectionString, engine);
            }
        }

        public void DownAll(string assemblyPath,string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoMigrationsForAssembly(assemblyPath, connectionString,MigrationDirection.Down);
                }
                else
                {
                    EasyMig.ToSqlServer.DoMigrationsForAssembly(assemblyPath, connectionString,MigrationDirection.Down);
                }
            }
            else
            {
                EasyMig.ToMySql.DoMigrationsForAssembly(assemblyPath, connectionString, engine,MigrationDirection.Down);
            }

        }

        public void UpOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoMigrationOnlyFor(fileName, assemblyPath, connectionString);
                }
                else
                {
                    EasyMig.ToSqlServer.DoMigrationOnlyFor(fileName, assemblyPath, connectionString);
                }
            }
            else
            {
                EasyMig.ToMySql.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, engine);
            }
        }

        public void DownOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, MigrationDirection.Down);
                }
                else
                {
                    EasyMig.ToSqlServer.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, MigrationDirection.Down);
                }
            }
            else
            {
                EasyMig.ToMySql.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, engine,MigrationDirection.Down);
            }
        }

        public void SeedAll(string assemblyPath, string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoSeedForAssembly(assemblyPath, connectionString);
                }
                else
                {
                    EasyMig.ToSqlServer.DoSeedForAssembly(assemblyPath, connectionString);
                }
            }
            else
            {
                EasyMig.ToMySql.DoSeedForAssembly(assemblyPath, connectionString, engine);
            }
        }

        public void SeedOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            if (this.IsSqlClient(providerName))
            {
                if (this.IsAttachDbFilename(connectionString))
                {
                    EasyMig.ToSqlServerAttachedDbFile.DoSeedOnlyFor(fileName, assemblyPath, connectionString);
                }
                else
                {
                    EasyMig.ToSqlServer.DoSeedOnlyFor(fileName, assemblyPath, connectionString);
                }
            }
            else
            {
                EasyMig.ToMySql.DoSeedOnlyFor(fileName, assemblyPath, connectionString, engine);
            }
        }

        public void Clear()
        {
            EasyMig.ClearMigrations();
            EasyMig.ClearSeeders();
        }
    }
}
