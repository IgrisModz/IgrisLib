using System.Windows.Input;

namespace IgrisLib.Mvvm
{
    /// <summary>
    /// The delegate command interface.
    /// </summary>
    public interface IDelegateCommand : ICommand
    {
        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
