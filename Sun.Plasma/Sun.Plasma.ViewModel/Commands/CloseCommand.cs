using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Drawing;

namespace Sun.Plasma.ViewModel.Commands
{
    public class CloseCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }        

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var window = Application.Current.Windows.OfType<Window>().Where(x => x.IsActive).FirstOrDefault();
            if (window.GetType().ToString().ToLower().Contains("login"))
                Application.Current.Shutdown();
            if (window.GetType().ToString().ToLower().Contains("settings"))
                window.Close();
            else
                window.Hide();
        }
    }
}
