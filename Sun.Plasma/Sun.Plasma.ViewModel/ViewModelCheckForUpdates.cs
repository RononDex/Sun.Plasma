using Sun.Core.SelfUpdating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.IO;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelCheckForUpdates : ViewModelBase
    {
        private bool? _updateAvailable;
        public bool? UpdateAvailable
        {
            get { return this._updateAvailable; }
            set { this._updateAvailable = value; OnPropertyChanged("UpdateAvailable"); }
        }

        private bool? _selfUpdaterUpdateAvailable;
        public bool? SelfUpdaterUpdateAvailable
        {
            get { return this._selfUpdaterUpdateAvailable; }
            set { this._selfUpdaterUpdateAvailable = value; OnPropertyChanged("SelfUpdaterUpdateAvailable"); }
        }

        /// <summary>
        /// Checks for available updates by starting a seperate thread
        /// </summary>
        public void CheckForUpdatesAsync()
        {
            var thread = new Thread(CheckForUpdatesEntryPoint);
            thread.Start();
        }

        private void CheckForUpdatesEntryPoint()
        {
            #if DEBUG
            return;
            #endif

            var updater = new SelfUpdater();

            var selfUpdaterVersion = AssemblyName.GetAssemblyName(Path.Combine(new string[]
                {
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 
                    "Sun.SelfUpdater",
                    SelfUpdater.UPDATER_FILE 
                })).Version;

            var selfUpdaterOutOfDate = updater.CheckForUpdates(SelfUpdater.UPDATER_APPNAME, SelfUpdater.SUN_UPDATER_ROOTURL, selfUpdaterVersion);
            if (selfUpdaterOutOfDate)
            {
                this.SelfUpdaterUpdateAvailable = true;
                return;
            }

            this.UpdateAvailable = updater.CheckForUpdates("Sun.Plasma", SelfUpdater.SUN_UPDATER_ROOTURL, Assembly.GetExecutingAssembly().GetName().Version);
        }

       
    }
}
