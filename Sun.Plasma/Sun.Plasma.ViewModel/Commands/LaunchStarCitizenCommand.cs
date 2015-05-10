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
            var key = Registry.CurrentUser.OpenSubKey("Software\\Cloud Imperium Games\\StarCitizen");
            return key != null && key.GetValue("DisplayIcon") != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var key = Registry.CurrentUser.OpenSubKey("Software\\Cloud Imperium Games\\StarCitizen");
            Process.Start(key.GetValue("DisplayIcon").ToString());
        }
    }
}
