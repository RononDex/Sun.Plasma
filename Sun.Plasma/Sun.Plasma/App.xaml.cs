using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.Net;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class PlasmaApp : Application, ISingleInstanceApp
    {
        /// <summary>
        /// This property is true, when the application was started to update the Sun.SelfUpdater
        /// </summary>
        public static bool UpdateSelfUpdater { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            PlasmaTools.Logger.DebugFormat("Starting up Sun.Plasma");

            base.OnStartup(e);

            PlasmaTrayIcon.InitializeTrayIcon();

            // Set static property to store the startup argument
            if (e.Args.Length == 1
                && e.Args[0] == "UpdateSelfUpdater")
                UpdateSelfUpdater = true;
            else
                UpdateSelfUpdater = false;

            // Start application using the correct window
            if (UpdateSelfUpdater)
            {
                PlasmaTools.Logger.InfoFormat("Application was started with the \"UpdateSelfUpdater\" parameter. Updating SelfUpdater.");
                var window = new UpdateSelfUpdater();
                window.Show();
            }
            else
            {
                var window = new CheckForUpdates();
                window.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            PlasmaTools.Logger.DebugFormat("Exiting Sun.Plasma");

            base.OnExit(e);

            PlasmaTrayIcon.Dispose();
        }

        #region ISingleInstanceApp Members
        /// <summary>
        /// This event gets called on the first instance when a second instance of this application starts
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            // When a second instance gets started, show the main form
            if (MainWindow.WindowState == WindowState.Minimized)
                MainWindow.WindowState = WindowState.Normal;

            Boolean topmost = MainWindow.Topmost;
            MainWindow.Show();
            MainWindow.Topmost = true;
            MainWindow.Topmost = topmost;

            return true;
        }
        #endregion  

        /// <summary>
        /// This event catches unvought exception
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (!e.Handled)
            {
                PlasmaTools.Logger.FatalFormat("Unhandled exception occured: {0} InnerException: {1}", e.Exception, e.Exception.InnerException);
                global::System.Windows.Forms.MessageBox.Show(string.Format("Unhandled exception: {0}", e.Exception.Message), "Unhandled exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                e.Handled = true;
            }
        }
    }
}
