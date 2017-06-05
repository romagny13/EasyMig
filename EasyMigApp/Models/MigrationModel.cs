using System.ComponentModel.DataAnnotations;

namespace EasyMigApp.Models
{
    public class MigrationModel : Validatable
    {
        private string assemblyPath;
        [Required(ErrorMessage = "Assembly path is required")]
        public string AssemblyPath
        {
            get { return assemblyPath; }
            set { this.Set(ref assemblyPath, value); }
        }

        private string connectionString;
        [Required(ErrorMessage = "Connection string is required for updating db")]
        public string ConnectionString
        {
            get { return connectionString; }
            set { this.Set(ref connectionString, value); }
        }

        private string providerName;
        [Required(ErrorMessage = "Provider name is required")]
        public string ProviderName
        {
            get { return providerName; }
            set { this.Set(ref providerName, value); }
        }

        private string engine;
        public string Engine
        {
            get { return engine; }
            set { this.Set(ref engine, value); }
        }
    }
}
