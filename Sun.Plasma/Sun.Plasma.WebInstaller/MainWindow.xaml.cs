using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Security.Cryptography.X509Certificates;

namespace Sun.Plasma.WebInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ROOT_URL = "http://systemsunitednavy.com/apps";

        FrameworkElement CtlMoveable;
        PlasmaWebInstaller installer = new PlasmaWebInstaller();
        Application updaterApp;
        Application mainApp;
        Paragraph docParagraph;

        public MainWindow()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();
            this.DataContext = this;
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            // Enable moving the window when clicking on the control CtlMoveable 
            this.CtlMoveable = (FrameworkElement)this.Template.FindName("CtlMoveable", this);
            if (null != this.CtlMoveable)
            {
                this.CtlMoveable.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveableClick);
            }

            System.Windows.Application.Current.MainWindow = this;

            var installerThread = new Thread(InstallerEntryPoint);
            installerThread.Start();
        }

        void InstallerEntryPoint()
        {
            var localDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Sun.Plasma");
            var selfUpdaterDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Sun.SelfUpdater");

            try
            {
                // Create application folders
                if (!Directory.Exists(localDirectory))
                    Directory.CreateDirectory(localDirectory);

                DeleteDirectoryContents(localDirectory);

                if (!Directory.Exists(selfUpdaterDirectory))
                    Directory.CreateDirectory(selfUpdaterDirectory);

                DeleteDirectoryContents(selfUpdaterDirectory);

                // Because this function is beeing called from a seperate thread,
                // we have to use the dispatcher to access UI elements.
                // In WPF only the STAThread (Starting thread) has access to those elements...
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate() {
                    TbxStatus.Document = new FlowDocument();
                    docParagraph = new Paragraph();
                    TbxStatus.Document.Blocks.Add(docParagraph);
                    docParagraph.Inlines.Add(new Run(string.Format("Installing {0} to directory {1}\r\n", PlasmaWebInstaller.UPDATER_APPNAME, selfUpdaterDirectory)));
                    docParagraph.Inlines.Add(new Run(string.Format("Downloading {0}\r\n", PlasmaWebInstaller.UPDATER_APPNAME)));
                    TbxStatus.ScrollToEnd();
                });

                // Download the updater application first
                updaterApp = installer.GetApplication(PlasmaWebInstaller.UPDATER_APPNAME, ROOT_URL);
                updaterApp.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(updaterApp_PropertyChanged);
                installer.DownloadApplication(updaterApp, selfUpdaterDirectory, ROOT_URL);

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    docParagraph.Inlines.Add(new Run(string.Format("\r\nInstalling {0} to directory {1}\r\n", PlasmaWebInstaller.APP_NAME, localDirectory)));
                    docParagraph.Inlines.Add(new Run(string.Format("Downloading {0}\r\n", PlasmaWebInstaller.APP_NAME)));
                    TbxStatus.ScrollToEnd();
                });

                // Download application
                mainApp = installer.GetApplication(PlasmaWebInstaller.APP_NAME, ROOT_URL);
                mainApp.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(application_PropertyChanged);
                installer.DownloadApplication(mainApp, localDirectory, ROOT_URL);

                #region // Install certificate in trusted publisher folder

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    docParagraph.Inlines.Add(new Run("\r\nInstalling digital certification in the trusted publisher store\r\n"));
                    TbxStatus.ScrollToEnd();
                });

                // Open the certificate from embedded resources (embedded resources are directly embedded into the assembly as binary stream)
                X509Certificate2 certificate = new X509Certificate2(Sun.Plasma.WebInstaller.Resources.SystemsUnitedNavyCertificate, "W0RKINSTAL");
                X509Store store = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine);

                store.Open(OpenFlags.ReadWrite);
                if (!store.Certificates.Contains(certificate))
                    store.Add(certificate);

                store.Close();

                #endregion

                #region // Create desktop icon
                string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                using (StreamWriter writer = new StreamWriter(deskDir + string.Format("\\{0}.url", PlasmaWebInstaller.APP_NAME)))
                {
                    string app = System.IO.Path.Combine(localDirectory, "Sun.Plasma.exe");
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=file:///" + app);
                    writer.WriteLine("IconIndex=0");
                    string icon = app.Replace('\\', '/');
                    writer.WriteLine("IconFile=" + icon);
                    writer.Flush();
                }

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    docParagraph.Inlines.Add(new Run("\r\nDesktop Icon created\r\n"));
                    docParagraph.Inlines.Add(new Run("Installation finished\r\n"));
                    TbxStatus.ScrollToEnd();
                });

                #endregion
            }
            catch (Exception ex)
            {
                global::System.Windows.Forms.MessageBox.Show(
                    string.Format("Error installing {1}:\r\n{0}\r\n\r\nThe installation has been canceled.\r\n!!! Please report this error in the S.U.N. forum !!!", ex.Message, PlasmaWebInstaller.APP_NAME),
                    "Error",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);

                Directory.Delete(localDirectory, true);

                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    App.Current.Shutdown();
                });
                return;
            }

            System.Windows.Forms.MessageBox.Show(string.Format("The installation of \"{0}\" has been completed.\r\nYou find a shortcut to start the application on your desktop.", PlasmaWebInstaller.APP_NAME),
                "Installation complete",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Asterisk);

            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
            {
                docParagraph.Inlines.Add(new Run("\r\nInstaller exits in 10 seconds..."));

                new Thread(delegate()
                {
                    Thread.Sleep(10000);
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        App.Current.Shutdown();
                    });
                }).Start();
            });
        }

        #region Events fired from download operation

        void updaterApp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateStatus")
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                     docParagraph.Inlines.Add(new Run(updaterApp.UpdateStatus + "\r\n"));
                     TbxStatus.ScrollToEnd();
                });
            }
        }

        void application_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateStatus")
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                     docParagraph.Inlines.Add(new Run(mainApp.UpdateStatus + "\r\n"));
                     TbxStatus.ScrollToEnd();
                });
            }
        }

        #endregion

        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void DeleteDirectoryContents(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
            {
                File.Delete(file);
            }
        }
    }
}
