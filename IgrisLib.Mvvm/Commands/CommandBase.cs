using System;

namespace IgrisLib.Mvvm
{
    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class CommandBase : IDelegateCommand
    {
        /// <summary>
        /// Invoked on can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Checks if the executeMethod can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes the executeMethod.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
