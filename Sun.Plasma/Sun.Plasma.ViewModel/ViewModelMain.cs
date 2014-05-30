using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Reflection;

namespace Sun.Plasma.ViewModel
{
    /// <summary>
    /// The viewmodel for the main window
    /// </summary>
    public class ViewModelMain : ViewModelBase
    {
        public ICommand CloseCommand
        {
            get { return new Commands.CloseCommand(); }
        }

        public ICommand MinimizeCommand
        {
            get { return new Commands.MinimizeCommand(); }
        }

        public ICommand ExitCommand
        {
            get { return new Commands.ExitCommand(); }
        }

        /// <summary>
        /// The version of the running application
        /// </summary>
        public string ApplicationVersion 
        {
            get 
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("S.U.N. Plasma v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }
    }
}
