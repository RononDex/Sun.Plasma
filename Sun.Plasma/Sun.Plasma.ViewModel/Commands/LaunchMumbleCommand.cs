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
            //TODO: Check if Mumble is installed
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            // TODO: Launch Mumble
            // http://wiki.mumble.info/wiki/Mumble_URL
        }
    }
}
