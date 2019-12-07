using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GroupKStegafy.Utility
{
    public class RelayCommand : ICommand
    {
        #region Data members

        private readonly Action<object> execute;

        private readonly Predicate<object> canExecute;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="RelayCommand" /> class.</summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Methods

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        /// <returns>EventHandler for a RelayCommand</returns>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            var result = this.canExecute?.Invoke(parameter) ?? true;
            return result;
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        ///     Precondition: none
        ///     Postcondition: none
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.execute(parameter);
            }
        }

        /// <summary>
        /// Calls the CanExecuteChanged event when an update happens enabling/disabling execution
        /// </summary>
        public void UpdateCanExecute() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
