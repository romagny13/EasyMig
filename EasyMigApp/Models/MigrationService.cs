using EasyMigLib;
using EasyMigLib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public void CreateMigrationScript(string assemblyPath, string providerName, string fileName, string engine)
        {
            EasyMig.CreateMigrationScript(assemblyPath, providerName, fileName, engine);
        }

        public void CreateSeedScript(string assemblyPath, string providerName, string fileName, string engine)
        {
            EasyMig.CreateSeedScript(assemblyPath, providerName, fileName, engine);
        }

        public void UpAll(string assemblyPath, string connectionString, string providerName, string engine)
        {
            EasyMig.DoMigrationsForAssembly(assemblyPath,connectionString, providerName, engine);
        }

        public void DownAll(string assemblyPath,string connectionString, string providerName, string engine)
        {
            EasyMig.DoMigrationsForAssembly(assemblyPath,connectionString, providerName, engine, MigrationDirection.Down);
        }

        public void UpOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            EasyMig.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, providerName, engine);
        }

        public void DownOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            EasyMig.DoMigrationOnlyFor(fileName, assemblyPath, connectionString, providerName, engine, MigrationDirection.Down);
        }

        public void SeedAll(string assemblyPath, string connectionString, string providerName, string engine)
        {
            EasyMig.DoSeedForAssembly(assemblyPath, connectionString, providerName, engine);
        }

        public void SeedOne(string fileName, string assemblyPath, string connectionString, string providerName, string engine)
        {
            EasyMig.DoSeedOnlyFor(fileName, assemblyPath, connectionString, providerName, engine);
        }
    }
}
