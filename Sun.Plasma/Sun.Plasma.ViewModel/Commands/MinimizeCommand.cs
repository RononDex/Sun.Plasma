using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace Sun.Plasma.ViewModel.Commands
{
    /// <summary>
    /// This command minimizes the current active window
    /// </summary>
    public class MinimizeCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var window = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            window.WindowState = WindowState.Minimized;
        }
    }
}
