using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using Sun.Plasma.ViewModel.Commands;

namespace Sun.Plasma.ViewModel
{
    /// <summary>
    /// The viewmodel for the main window
    /// </summary>
    public class ViewModelMain : ViewModelBase
    {
        #region Commands

        public ICommand CloseCommand
        {
            get { return new CloseCommand(); }
        }

        public ICommand MinimizeCommand
        {
            get { return new MinimizeCommand(); }
        }

        public ICommand ExitCommand
        {
            get { return new ExitCommand(); }
        }

        public ICommand LaunchStarCitizenCommand
        {
            get { return new LaunchStarCitizenCommand(); }
        }

        public ICommand LaunchMumbleCommand
        {
            get { return new LaunchMumbleCommand() ; }
        }

        public ICommand OpenSettingsCommand
        {
            get { return new OpenSettingsCommand(SettingsWindow); }
        }

        #endregion

        public Window SettingsWindow { get; set; }

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
