using System;
using System.Windows.Input;

namespace IgrisLib.Mvvm
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        //public void RaiseCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, new EventArgs());
        //}
    }


    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            if (!(parameter is T))
            {
                return false;
            }
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (parameter is T)
            {
                _execute?.Invoke((T)parameter);
            }
        }

        //public void RaiseCanExecuteChanged()
        //{
        //    CanExecuteChanged?.Invoke(this, new EventArgs());
        //}
    }
}
