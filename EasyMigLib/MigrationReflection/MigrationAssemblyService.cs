using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace EasyMigLib.MigrationReflection
{

    public class MigrationAssemblyService : IMigrationAssemblyService
    {

        public void SortTypes(List<RecognizedMigrationFile> types)
        {
            types.Sort((next, a) =>
            {
                if (a.Version != null && next.Version != null)
                {
                    if (a.Name == next.Name)
                    {
                        return next.Version.CompareTo(a.Version);
                    }
                    else
                    {
                        return next.Name.CompareTo(a.Name);
                    }
                }
                return next.FullName.CompareTo(a.FullName);
            });
        }

        public Dictionary<string, List<RecognizedMigrationFile>> Group(List<RecognizedMigrationFile> orderedList)
        {
            var result = new Dictionary<string, List<RecognizedMigrationFile>>();
            foreach (var type in orderedList)
            {
                if (!result.ContainsKey(type.Name))
                {
                    result[type.Name] = new List<RecognizedMigrationFile>();
                }
                if (type.Version == null)
                {
                    // first
                    result[type.Name].Insert(0, type);
                }
                else
                {
                    result[type.Name].Add(type);
                }
            }
            return result;
        }

        public RecognizedMigrationFile GetRecognizedMigrationFile(Type type)
        {
            var regex = new Regex("^([0-9_]+)([a-zA-Z]+)$");
            if (regex.IsMatch(type.Name))
            {
                var matches = regex.Matches(type.Name);
                return new RecognizedMigrationFile(type,type.Name, matches[0].Groups[2].Value, matches[0].Groups[1].Value);
            }
            else
            {
                return new RecognizedMigrationFile(type, type.Name, type.Name);
            }
        }

        public List<RecognizedMigrationFile> FindTypes<T>(Type[] assemblyTypes)
        {
            var result = new List<RecognizedMigrationFile>();
            var types = assemblyTypes.Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract);
            foreach (var type in types)
            {
                result.Add(this.GetRecognizedMigrationFile(type));
            }
            this.SortTypes(result);

            return result;
        }

        public Assembly LoadAssembly(string path)
        {
            return Assembly.LoadFile(path);
        }

        public List<RecognizedMigrationFile> FindTypes<T>(string assemblyPath)
        {
            var assembly = this.LoadAssembly(assemblyPath);
            var types = assembly.GetTypes();
            return this.FindTypes<T>(types);
        }

        public RecognizedMigrationFile FindType<T>(string assemblyPath, string matchName)
        {
            var assembly = this.LoadAssembly(assemblyPath);
            var types = assembly.GetTypes();
            return this.FindType<T>(types, matchName);
        }

        public RecognizedMigrationFile FindType<T>(Type[] assemblyTypes, string matchName)
        {
            var type = assemblyTypes
                .Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract && t.Name == matchName)
                .FirstOrDefault();

            return this.GetRecognizedMigrationFile(type);
        }

        public T CreateInstance<T>(Type type)
        {
            return (T)Activator.CreateInstance(type);
        }

        public void RunMigration(RecognizedMigrationFile recognizedType, MigrationDirection direction = MigrationDirection.Up)
        {
            var migration = this.CreateInstance<Migration>(recognizedType.Type);
            if (direction == MigrationDirection.Up)
            {
                migration.Up();
            }
            else
            {
                migration.Down();
            }
        }

        public void RunSeeder(RecognizedMigrationFile recognizedType)
        {
            var seeder = this.CreateInstance<Seeder>(recognizedType.Type);
            seeder.Run();
        }
    }
}
