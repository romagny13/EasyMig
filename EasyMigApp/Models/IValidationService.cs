using System.Collections.Generic;

namespace EasyMigApp.Models
{
    public interface IValidationService
    {
        ValidationAllServiceResult ClearAll(object model);
        ValidationServiceResult ValidateProperty(object model, string name);
        ValidationAllServiceResult ValidateAll(object model);
    }


    public class ValidationAllServiceResult
    {
        public bool HasError { get; }
        public Dictionary<string, ValidationServiceResult> Results { get; }
        public ValidationAllServiceResult(bool hasError, Dictionary<string, ValidationServiceResult> results)
        {
            this.HasError = hasError;
            this.Results = results;
        }
    }
    public class ValidationServiceResult
    {
        public bool HasError { get; }
        public List<string> Errors { get; }
        public ValidationServiceResult(bool hasError, List<string> errors = null)
        {
            this.HasError = hasError;
            this.Errors = errors;
        }
    }
}
