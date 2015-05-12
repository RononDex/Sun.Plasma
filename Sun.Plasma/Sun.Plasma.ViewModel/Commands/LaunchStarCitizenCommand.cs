using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sun.Plasma.ViewModel.Commands
{
    /// <summary>
    /// Launches the star citizen launcher
    /// </summary>
    public class LaunchStarCitizenCommand : ICommand
    {

        public bool CanExecute(object parameter)
        {
            // If Star Citizen is installed, there should be
            // a registry entry located in CurrentUser
            var key = Registry.CurrentUser.OpenSubKey("Software\\Cloud Imperium Games\\StarCitizen");
            return key != null && key.GetValue("DisplayIcon") != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // Read the path of where Star Citizen is installed
            // on the system and launch the binary
            var key = Registry.CurrentUser.OpenSubKey("Software\\Cloud Imperium Games\\StarCitizen");
            Process.Start(key.GetValue("DisplayIcon").ToString());
        }
    }
}
