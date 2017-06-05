using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EasyMigApp.Models
{
    public class Validatable : Observable, INotifyDataErrorInfo
    {
        protected Dictionary<string, List<string>> container;
        protected List<string> emptyList;
        protected IValidationService validationService;

        public Validatable()
            : this(new DataAnnotationsValidationService())
        { }

        public Validatable(IValidationService validationService)
        {
            this.container = new Dictionary<string, List<string>>();
            this.emptyList = new List<string>();
            this.validationService = validationService;
        }

        public bool PropertyHasErrors(string propertyName)
        {
            return this.container.ContainsKey(propertyName);
        }

        public void SetPropertyErrors(string propertyName, List<string> errors)
        {
            this.container[propertyName] = errors;
            this.RaiseErrorsChanged(propertyName);
        }

        public void ClearPropertyErrors(string propertyName)
        {
            this.container.Remove(propertyName);
            this.RaiseErrorsChanged(propertyName);
        }

        public List<string> GetPropertyErrors(string propertyName)
        {
            return this.container[propertyName];
        }

        public ValidationServiceResult ValidateProperty(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) { throw new Exception("Property name required"); }

            var result = this.validationService.ValidateProperty(this, propertyName);
            if (result.HasError)
            {
                this.SetPropertyErrors(propertyName, result.Errors);
            }
            else if (this.PropertyHasErrors(propertyName))
            {
                this.ClearPropertyErrors(propertyName);
            }
            return result;
        }

        public void ClearErrors()
        {
            var result = this.validationService.ClearAll(this);
            foreach (var propertyResult in result.Results)
            {
                this.ClearPropertyErrors(propertyResult.Key);
            }
        }

        public ValidationAllServiceResult ValidateAll()
        {
            var result = this.validationService.ValidateAll(this);
            foreach (var propertyResult in result.Results)
            {
                if (propertyResult.Value.HasError)
                {
                    this.SetPropertyErrors(propertyResult.Key, propertyResult.Value.Errors);
                }
                else if (this.PropertyHasErrors(propertyResult.Key))
                {
                    this.ClearPropertyErrors(propertyResult.Key);
                }
            }
            return result;
        }

        protected override bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            var result = base.Set(ref storage, value, propertyName);
            if (result && !string.IsNullOrEmpty(propertyName))
            {
                if (this.ValidationMode == ValidationMode.PropertyChanged)
                {
                    this.ValidateProperty(propertyName);
                }
            }
            return result;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName) && this.PropertyHasErrors(propertyName))
            {
                return this.GetPropertyErrors(propertyName);
            }
            return this.emptyList;
        }

        public ValidationMode ValidationMode { get; set; }

        /// <summary>
        /// Check if the model have one or more properties in error
        /// </summary>
        public bool HasErrors
        {
            get { return container.Count > 0; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    public enum ValidationMode
    {
        PropertyChanged,
        Submit
    }
}