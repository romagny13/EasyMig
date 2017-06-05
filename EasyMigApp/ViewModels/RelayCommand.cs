using System;
using System.Windows.Input;

namespace EasyMigApp.ViewModels
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return this.EvaluateCondition(parameter);
        }

        public void Execute(object parameter)
        {
            this.InvokeCallback(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public abstract bool EvaluateCondition(object parameter);
        public abstract void InvokeCallback(object parameter);
    }

    public class RelayCommand : CommandBase
    {
        protected Action callback;
        protected Func<bool> condition;

        public RelayCommand(Action callback, Func<bool> condition = null)
        {
            this.callback = callback;
            this.condition = condition;
        }

        public override bool EvaluateCondition(object parameter)
        {
            return this.condition == null ? true : this.condition();
        }

        public override void InvokeCallback(object parameter)
        {
            this.callback();
        }
    }

    public class RelayCommand<T> : CommandBase
    {
        protected Action<T> callback;
        protected Func<T, bool> condition;

        public RelayCommand(Action<T> callback, Func<T, bool> condition = null)
        {
            this.callback = callback;
            this.condition = condition;
        }

        public override bool EvaluateCondition(object parameter)
        {
            return this.condition == null ? true : this.condition((T)parameter);
        }

        public override void InvokeCallback(object parameter)
        {
            this.callback((T)parameter);
        }
    }
}
