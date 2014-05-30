using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Sun.SelfUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class SelfUpdater : Application
    {
        public static string ApplicationName { get; set; }
        public static string ApplicationDirectory { get; set; }
        public static string RootUrl { get; set; }
        public static string ApplicationToStartAfterUpdate { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            /*
            * Parse arguments, that have been passed to this application
            * You have to start this application by passing 4 arguments:
            * Sun.SelfUpdater.exe <ApplicationName> <ApplicationDirectory> <RootUrl> <ApplicaionToStartAfterUpdate>
            */

            // Shutdown application if there are not 4 arguments
            if (e.Args.Length != 4)
            {
                this.Shutdown();
                return;
            }

            // First parameter is the application name
            ApplicationName = e.Args[0];

            // Second parameter is the file path, where the application is located locally
            ApplicationDirectory = e.Args[1];

            // Third parameter is the rootUrl, where the application lays online
            RootUrl = e.Args[2];

            // Fourth parameter is the application that has to be started after the update has been completed
            ApplicationToStartAfterUpdate = e.Args[3];
        }

        /// <summary>
        /// This event catches unvought exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                global::System.Windows.Forms.MessageBox.Show(string.Format("Error: {0}", e.Exception.Message), "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                e.Handled = true;
                this.Shutdown();
            }
        }
    }
}
