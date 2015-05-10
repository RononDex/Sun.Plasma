using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sun.Plasma.ViewModel.Commands
{
    /// <summary>
    /// Opens the Star Citizen launcher
    /// </summary>
    public class LaunchStarCitizen : ICommand
    {
        public bool CanExecute(object parameter)
        {
            // TODO: Check if the star citizen launcher is installed
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            

        }
    }
}
