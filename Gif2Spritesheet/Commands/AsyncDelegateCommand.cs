﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gif2Spritesheet.Commands
{
    /// <summary>
    /// Provides an <see cref="ICommand"/> implementation which relays the <see cref="Execute"/> and <see cref="CanExecute"/> 
    /// method to the specified delegates.
    /// This implementation disables the command during the async command execution.
    /// </summary>
    public class AsyncDelegateCommand : ICommand
    {
        private readonly Func<object, Task> execute;
        private readonly Func<object, bool> canExecute;
        private bool isExecuting;

        /// <summary>Initializes a new instance of the <see cref="AsyncDelegateCommand"/> class.</summary>
        /// <param name="execute">Async Delegate to execute when Execute is called on the command.</param>
        /// <exception cref="ArgumentNullException">The execute argument must not be null.</exception>
        public AsyncDelegateCommand(Func<Task> execute) : this(execute, null) { }

        /// <summary>Initializes a new instance of the <see cref="AsyncDelegateCommand"/> class.</summary>
        /// <param name="execute">Async Delegate to execute when Execute is called on the command.</param>
        /// <exception cref="ArgumentNullException">The execute argument must not be null.</exception>
        public AsyncDelegateCommand(Func<object, Task> execute) : this(execute, null) { }

        /// <summary>Initializes a new instance of the <see cref="AsyncDelegateCommand"/> class.</summary>
        /// <param name="execute">Async Delegate to execute when Execute is called on the command.</param>
        /// <param name="canExecute">Delegate to execute when CanExecute is called on the command.</param>
        /// <exception cref="ArgumentNullException">The execute argument must not be null.</exception>
        public AsyncDelegateCommand(Func<Task> execute, Func<bool> canExecute)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            this.execute = p => execute();
            this.canExecute = canExecute == null ? null : p => canExecute!();
        }

        /// <summary>Initializes a new instance of the <see cref="AsyncDelegateCommand"/> class.</summary>
        /// <param name="execute">Async Delegate to execute when Execute is called on the command.</param>
        /// <param name="canExecute">Delegate to execute when CanExecute is called on the command.</param>
        /// <exception cref="ArgumentNullException">The execute argument must not be null.</exception>
        public AsyncDelegateCommand(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        /// <summary>Returns a disabled command.</summary>
        public static AsyncDelegateCommand DisabledCommand { get; } = new AsyncDelegateCommand(() => Task.CompletedTask, () => false);

        private bool IsExecuting
        {
            get => isExecuting;
            set
            {
                if (isExecuting != value)
                {
                    isExecuting = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>Occurs when changes occur that affect whether or not the command should execute.</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>Defines the method that determines whether the command can execute in its current state.</summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (canExecute == null || canExecute(parameter));
        }

        /// <summary>Defines the method to be called when the command is invoked.</summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            IsExecuting = true;
            try
            {
                await execute(parameter);
            }
            finally
            {
                IsExecuting = false;
            }
        }

        /// <summary>Raises the <see cref="CanExecuteChanged"/> event.</summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }

        /// <summary>Raises the <see cref="CanExecuteChanged"/> event.</summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
