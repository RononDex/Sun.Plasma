using Sun.Core.SelfUpdating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sun.Plasma.ViewModel
{
    /// <summary>
    /// View Model for the info screen
    /// </summary>
    public class ViewModelApplicationIInfo : ViewModelBase
    {
        /// <summary>
        /// The current installed version of Plasma
        /// </summary>
        public string PlasmaVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
       } 

        /// <summary>
        /// The current installed version of SelfUpdater
        /// </summary>
        public string SelfUpdaterVersion
        {
            get
            {
                var selfUpdaterVersion = AssemblyName.GetAssemblyName(Path.Combine(new string[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 
                    "Sun.SelfUpdater",
                    SelfUpdater.UPDATER_FILE
                })).Version;
                return string.Format("v{0}.{1}.{2}", selfUpdaterVersion.Major, selfUpdaterVersion.Minor, selfUpdaterVersion.Build);
            }
        }
    }
}
