using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sun.Plasma.ViewModel.Commands
{
    public class LaunchMumbleCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            // If mumble is installed, there should be the "Mumble Url"
            // registered in the Windows Registry (ClassesRoot)
            var key = Registry.ClassesRoot.OpenSubKey("mumble");
            return key != null;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // TODO: Launch Mumble
            // http://wiki.mumble.info/wiki/Mumble_URL
        }
    }
}
