using System;

namespace IgrisLib.Mvvm
{
    /// <summary>
    /// DelegateCommand is an implementation of ICommand that allows to invoke manually CanExecuteChanged (does not use the CommandManager unlike the RoutedCommands).
    /// </summary>
    public class DelegateCommand : CommandBase
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Creates the <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        /// <param name="canExecute">The method used to check if <see cref="Execute(object)"/> can be invoked</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Creates the <see cref="DelegateCommand"/>.
        /// </summary>
        /// <param name="execute">The method to execute</param>
        public DelegateCommand(Action execute) : this(execute, () => true)
        {
        }

        private DelegateCommand()
        {
        }

        /// <summary>
        /// Invokes the <see cref="_execute"/>.
        /// </summary>
        /// <param name="parameter">The parameter is not used</param>
        public override void Execute(object parameter)
        {
            _execute?.Invoke();
        }

        /// <summary>
        /// Checks if <see cref="Execute(object)"/> can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter is not used</param>
        /// <returns>True if command have to be executed</returns>
        public override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }
    }
}
