using System;

namespace IgrisLib.Mvvm
{
    /// <summary>
    /// DelegateCommand is an implementation of ICommand that allows to invoke manually CanExecuteChanged (does not use the CommandManager unlike the RoutedCommands). DelegateCommand with parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    public class DelegateCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        /// Creates the <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        /// <param name="canExecute">The method used to check if <see cref="Execute(object)"/> can be invoked</param>
        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Creates the <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        public DelegateCommand(Action<T> execute) : this(execute, p => true)
        {
        }

        private DelegateCommand()
        {
        }

        /// <summary>
        /// Checks if <see cref="Execute(object)"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public override void Execute(object parameter)
        {
            if (parameter is T t)
            {
                _execute?.Invoke(t);
            }
        }

        /// <summary>
        /// Invokes the <see cref="_execute"/>.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public override bool CanExecute(object parameter)
        {
            return parameter is T t && (_canExecute == null || _canExecute(t));
        }
    }
}
