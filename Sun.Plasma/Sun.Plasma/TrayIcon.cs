using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sun.Plasma
{
    /// <summary>
    /// Holds the tray icon for the Plasma Application
    /// </summary>
    static class PlasmaTrayIcon
    {
        public static System.Windows.Forms.NotifyIcon NotifyIcon { set; get; }
        static System.Windows.Forms.ContextMenu NotifyContextMenu { set; get; }

        /// <summary>
        /// Initializes the trayicon and the contextmenu
        /// </summary>
        public static void InitializeTrayIcon()
        {
            // Initialize objects and tray icon
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
            NotifyIcon.Text = "Sun Plasma";
            NotifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            NotifyIcon.DoubleClick += new EventHandler(NotifyIcon_Click);
            NotifyIcon.Visible = true;

            // Set up the context menu for the notify icon
            NotifyContextMenu = new System.Windows.Forms.ContextMenu();

            // Launch Star Citizen
            var launchStarCitizenItem = NotifyContextMenu.MenuItems.Add("Launch Star Citizen");
            var command = new ViewModel.Commands.LaunchStarCitizenCommand();
            launchStarCitizenItem.Enabled = command.CanExecute(null);
            launchStarCitizenItem.Click += (sender, args) => command.Execute(null);

            // Launch Mumble
            var launchMumble = NotifyContextMenu.MenuItems.Add("Launch Mumble");
            var commandMumble = new ViewModel.Commands.LaunchMumbleCommand();
            launchMumble.Enabled = commandMumble.CanExecute(null);
            launchMumble.Click += (sender, args) => commandMumble.Execute(null);

            // Exit
            NotifyContextMenu.MenuItems.Add("-");
            NotifyContextMenu.MenuItems.Add("Exit", NotifyContextMenuExit_Click);

            NotifyIcon.ContextMenu = NotifyContextMenu;
        }

        /// <summary>
        /// Disposes the Tray Icon
        /// </summary>
        public static void Dispose()
        {
            if (NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
            }
        }

        #region Click Events

        static void NotifyIcon_Click(object sender, EventArgs e)
        {
            PlasmaTools.Logger.DebugFormat("Bringing Sun.Plasma back from task bar");
            var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.GetType().ToString().Contains("MainWindow"));
            if (window != null)
                window.Show();
        }

        static void NotifyContextMenuExit_Click(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            NotifyIcon.Dispose();
            Application.Current.Shutdown();
        }

        #endregion
    }
}
