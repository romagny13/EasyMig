using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace EasyMigApp.Models
{
    public class DataAnnotationsValidationService : IValidationService
    {
        protected List<string> emptyList;

        public DataAnnotationsValidationService()
        {
            this.emptyList = new List<string>();
        }

        public PropertyInfo GetPropertyInfo(Type type, string name)
        {
            return type.GetProperty(name);
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties().Where(p => p.CanRead && p.CanWrite);
        }

        public object GetPropertyValue(object model, PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(model);
        }

        public List<string> GetErrors(List<ValidationResult> validationResults)
        {
            var result = new List<string>();
            foreach (var validationResult in validationResults)
            {
                result.Add(validationResult.ErrorMessage);
            }
            return result;
        }

        public ValidationServiceResult ValidateProperty(object model, PropertyInfo propertyInfo)
        {
            var validationResults = new List<ValidationResult>();
            var value = this.GetPropertyValue(model, propertyInfo);
            var context = new ValidationContext(model, null, null) { MemberName = propertyInfo.Name };

            if (!Validator.TryValidateProperty(value, context, validationResults))
            {
                var errors = this.GetErrors(validationResults);
                return new ValidationServiceResult(true, errors);
            }
            else
            {
                return new ValidationServiceResult(false, this.emptyList);
            }
        }

        public ValidationServiceResult ValidateProperty(object model, string name)
        {
            var propertyInfo = this.GetPropertyInfo(model.GetType(), name);
            if (propertyInfo == null) { throw new Exception("No property found for " + name); }
            return this.ValidateProperty(model, propertyInfo);
        }

        public ValidationAllServiceResult ValidateAll(object model)
        {
            bool hasError = false;
            var results = new Dictionary<string, ValidationServiceResult>();
            var propertyInfos = this.GetProperties(model.GetType());
            foreach (var propertyInfo in propertyInfos)
            {
                var propertyResult = this.ValidateProperty(model, propertyInfo);
                if (propertyResult.HasError)
                {
                    hasError = true;
                }
                results[propertyInfo.Name] = propertyResult;
            }

            return new ValidationAllServiceResult(hasError, results);
        }

        public ValidationAllServiceResult ClearAll(object model)
        {
            var results = new Dictionary<string, ValidationServiceResult>();
            var propertyInfos = this.GetProperties(model.GetType());
            foreach (var propertyInfo in propertyInfos)
            {
                results[propertyInfo.Name] = new ValidationServiceResult(false, this.emptyList);
            }
            return new ValidationAllServiceResult(false, results);
        }
    }
}