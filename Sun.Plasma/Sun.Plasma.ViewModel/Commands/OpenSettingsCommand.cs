using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sun.Plasma.ViewModel.Commands
{
    public class OpenSettingsCommand : ICommand
    {
        public Window SettingsWindow { get; set; }

        public OpenSettingsCommand(Window settingsWindow)
        {
            this.SettingsWindow = settingsWindow;
        }

        public bool CanExecute(object parameter)
        {
            // Can always be executed
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            SettingsWindow = Activator.CreateInstance(SettingsWindow.GetType()) as Window;
            SettingsWindow.ShowDialog();
        }
    }
}
