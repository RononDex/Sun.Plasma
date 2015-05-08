using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaktionslogik für CheckForUpdates.xaml
    /// </summary>
    public partial class CheckForUpdates : Window
    {
        FrameworkElement CtlMoveable;
        ViewModel.ViewModelCheckForUpdates vm;

        public CheckForUpdates()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();
            vm = new ViewModel.ViewModelCheckForUpdates();
            this.DataContext = vm;
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            // Enable moving the window when clicking on the control CtlMoveable 
            this.CtlMoveable = (FrameworkElement)this.Template.FindName("CtlMoveable", this);
            if (null != this.CtlMoveable)
            {
                this.CtlMoveable.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveableClick);
            }

            Application.Current.MainWindow = this;

            //// Disable Minimize and close buttons
            //var BtnClose = (FrameworkElement)this.Template.FindName("BtnClose", this);
            //if (BtnClose != null)
            //    BtnClose.Visibility = System.Windows.Visibility.Hidden;

            //var BtnMinimize = (FrameworkElement)this.Template.FindName("BtnMinimize", this);
            //if (BtnMinimize != null)
            //    BtnMinimize.Visibility = System.Windows.Visibility.Hidden;

            // Check for updates async
            // and register an event to catch its answer
            vm.PropertyChanged += vm_PropertyChanged;            
            vm.CheckForUpdatesAsync();
        }

        private delegate void showLoginScreen();
        private delegate void ApplicationShutdown();

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateAvailable")
            {
                if (vm.UpdateAvailable != null
                    && !vm.UpdateAvailable.Value)
                {
                    // If no update available, show login screen
                    // It is necessary to do this over the dispatcher, because
                    // this event is not getting called from the STAThread, but
                    // from the started async thread...
                    this.Dispatcher.Invoke(new showLoginScreen(ShowLoginScreen));
                    PlasmaTools.Logger.DebugFormat("No new version of Sun.Plasma found");
                }
                else
                {
                    // If update is available, start SelfUpdater.exe
                    var selfUpdaterPath = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                            "Sun.SelfUpdater",
                            Core.SelfUpdating.SelfUpdater.UPDATER_FILE);

                    PlasmaTools.Logger.InfoFormat("New version of Sun.Plasma found, starting SelfUpdater.exe");

                    // Check if self Updater exists
                    if (!File.Exists(selfUpdaterPath))
                    {
                        PlasmaTools.Logger.FatalFormat("Could not find the self updater executable!");
                        System.Windows.Forms.MessageBox.Show("Could not find the self updater executable.\r\nPlease reinstall the application to fix this issue!", "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        this.Dispatcher.Invoke(new ApplicationShutdown(ShutdownApplication));
                        return;
                    }

                    var updatePath = System.IO.Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                            "Sun.Plasma",
                            "Sun.Plasma.exe"
                        );

                    var startInfo = new ProcessStartInfo(selfUpdaterPath,
                        string.Format("{0} \"{1}\" {2} \"{3}\"",
                        "Sun.Plasma",
                        updatePath,
                        Sun.Core.SelfUpdating.SelfUpdater.SUN_UPDATER_ROOTURL,
                        Assembly.GetExecutingAssembly().Location));
                    try
                    {
                        Process.Start(startInfo);
                    }
                    catch { /* When the user declines the run as admin privileges, exception is thrown. In this case ignore it */ }
                    finally
                    {
                        // Exit this application
                        this.Dispatcher.Invoke(new ApplicationShutdown(ShutdownApplication));
                    }
                }
            }
            else if (e.PropertyName == "SelfUpdaterUpdateAvailable")
            {
                if (vm.SelfUpdaterUpdateAvailable != null
                    && vm.SelfUpdaterUpdateAvailable.Value)
                {
                    PlasmaTools.Logger.InfoFormat("New version of Sun.SelfUpdater found, restaring Sun.Plasma as admin");

                    // Make sure that Sun.StartAsAdmin.exe exists
                    if (!File.Exists("Sun.StartAsAdmin.exe"))
                    {
                        throw new Exception("Could not find file \"Sun.StartAsAdmin.exe\". Please reinstall Sun.Plasma to fix this issue");
                    }

                    // Try to start as admin
                    try
                    {
                        Process.Start("Sun.StartAsAdmin.exe", "\"" + Assembly.GetExecutingAssembly().Location + "\" UpdateSelfUpdater");
                    }
                    catch
                    {
                        // If user refuses admin rights, exception is thrown
                        // In this case do nothing and just shutdown the application
                    }

                    // Shut application down
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        Application.Current.Shutdown();
                    });
                }
            }
        }

        private void ShowLoginScreen()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
