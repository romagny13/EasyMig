using System;

namespace EasyMigLib.MigrationReflection
{
    public class RecognizedMigrationFile
    {
        // _1_
        public string Version { get; }

        // MY_MIGRATION
        public string Name { get; }

        // _1_MY_MIGRATION
        public string FullName { get; }

        // Migration | Seeder non abstract
        public Type Type { get; }

        public RecognizedMigrationFile(Type type, string fullName, string name, string version = null)
        {
            this.Version = version;
            this.Name = name;
            this.FullName = fullName;
            this.Type = type;
        }
    }

}
