using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Sun.Plasma.ViewModel.Commands
{
    /// <summary>
    /// The exit command exists the current application by shutting it down
    /// </summary>
    public class ExitCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // Shutdown the current application
            Application.Current.Shutdown();
        }
    }
}
