using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sun.Core.SelfUpdating;
using System.IO;
using System.Threading;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelUpdateSelfUpdater : ViewModelBase
    {
        /// <summary>
        /// The application object of the SelfUpdater application
        /// </summary>
        public Application SelfUpdaterApp { get; set; }

        /// <summary>
        /// Loads the application information from the webserver
        /// </summary>
        public void LoadApplicationData()
        {
            var updater = new SelfUpdater();
            this.SelfUpdaterApp = updater.GetApplication(SelfUpdater.UPDATER_APPNAME, SelfUpdater.SUN_UPDATER_ROOTURL);
        }

        /// <summary>
        /// Updates the selfupdater to the latest version using a seperate thread
        /// </summary>
        public void UpdateSelfUpdaterAsync()
        {
            var thread = new Thread(UpdateSelfUpdater);
            thread.Start();
        }

        /// <summary>
        /// Updates the SelfUpdater to the latest version
        /// </summary>
        private void UpdateSelfUpdater()
        {
            var updater = new SelfUpdater();
            var selfUpdaterPath = Path.Combine(new string[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 
                "Sun.SelfUpdater",
                "Sun.SelfUpdater.exe"
            });

            // Make sure the application folder exists
            if (!Directory.Exists(Path.GetDirectoryName(selfUpdaterPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(selfUpdaterPath));

            updater.UpdateApplication(SelfUpdaterApp, selfUpdaterPath, SelfUpdater.SUN_UPDATER_ROOTURL);
        }
    }
}
