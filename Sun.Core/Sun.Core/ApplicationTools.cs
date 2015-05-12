using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Sun.Core
{
    /// <summary>
    /// Holds some useful functions for applications
    /// </summary>
    public static class ApplicationTools
    {
        /// <summary>
        /// Checks if the given application is registered for auto-start
        /// </summary>
        /// <param name="appName"></param>
        public static bool IsAppRegisteredToLaunchOnStartUp(string appName)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            return (rkApp.GetValue(appName) != null);
        }

        /// <summary>
        /// Registers or deregisters an application from automatically starting when pc starts up
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="autoStart"></param>
        /// <param name="executablePath"></param>
        public static void SetAppRegisteredToLaunchOnStartup(string appName, bool autoStart, string executablePath)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (autoStart)
                rkApp.SetValue(appName, executablePath);
            else
                rkApp.DeleteValue(appName, false);

            CoreTools.Logger.InfoFormat("Set the autostart of application \"{0}\" to {1}", executablePath, autoStart);
        }
    }
}
