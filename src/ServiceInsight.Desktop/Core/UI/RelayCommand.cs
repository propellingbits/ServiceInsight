﻿namespace Particular.ServiceInsight.Desktop.Core.UI
{
    using System;
    using System.Windows.Input;

    public class RelayCommand : ICommand
    {
        Func<bool> canExecute;
        Action execute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null) throw new ArgumentNullException("execute");
            this.execute = execute;
            this.canExecute = canExecute;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public virtual bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute();
        }

        public virtual void Execute(object parameter)
        {
            execute();
        }
    }
}