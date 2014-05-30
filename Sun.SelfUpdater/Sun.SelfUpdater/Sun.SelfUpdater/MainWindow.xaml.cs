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
using System.ComponentModel;
using System.Reflection;
using System.Diagnostics;
using Sun.Core.SelfUpdating;
using System.Threading;
using System.Windows.Threading;

namespace Sun.SelfUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        FrameworkElement CtlMoveable;

        /// <summary>
        /// The version of the selfUpdater
        /// </summary>
        public string UpdaterVersion
        {
            get
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return string.Format("Sun.SelfUpdater v{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
        }

        private Sun.Core.SelfUpdating.SelfUpdater _updater;
        public Sun.Core.SelfUpdating.SelfUpdater Updater
        {
            get { return this._updater; }
            set { this._updater = value; OnPropertyChanged("Updater"); }
        }

        private Sun.Core.SelfUpdating.Application _updaterApplication;
        public Sun.Core.SelfUpdating.Application UpdaterApplication
        {
            get { return this._updaterApplication; }
            set { this._updaterApplication = value; OnPropertyChanged("UpdaterApplication"); }
        }

        private string _errorMsg;
        public string ErrorMsg
        {
            get { return this._errorMsg; }
            set { this._errorMsg = value; this.LblError.Content = value; }
        }

        public MainWindow()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();
            this.DataContext = this;


            // Setup changelog in RTF
            this.TbxStatus.Document = new FlowDocument();
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

            this.LblTitle.Content = string.Format("Updating {0}", SelfUpdater.ApplicationName);

            new Thread(UpdaterEntryPoint).Start();
        }

        void UpdaterEntryPoint()
        {
            // First try to download the application information from the server
            try
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                   {
                       this.LblStatus.Content = "Getting application information from server";
                   });
                this.Updater = new Sun.Core.SelfUpdating.SelfUpdater();
                this.UpdaterApplication = Updater.GetApplication(Sun.SelfUpdater.SelfUpdater.ApplicationName, Sun.SelfUpdater.SelfUpdater.RootUrl);
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    ErrorMsg = string.Format("Error: {0}", ex.Message);
                });
            }

            // If application information could be downloaded start updating
            if (this.UpdaterApplication != null)
            {
                // Display Changelog for all versions
                UpdaterApplication.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(updaterApp_PropertyChanged);
                foreach (var version in this.UpdaterApplication.GetAllVersionsFromChangeLog())
                {
                    this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                    {
                        this.TbxStatus.Document.Blocks.Add(new Paragraph(new Bold(new Run(string.Format("Version {0}", version)))));
                        var ruler = new Line { X1 = 0, Y1 = 0, X2 = 1000, Y2 = 0, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 2 };


                        var newStuffs = UpdaterApplication.GetAllNewStuffFromVersion(new Version(version));
                        if (newStuffs.Count > 0)
                        {
                            this.TbxStatus.Document.Blocks.Add(new Paragraph(new Run("New:")));
                            var newStuffList = new List();
                            foreach (var newStuff in newStuffs)
                            {
                                newStuffList.ListItems.Add(new ListItem(new Paragraph(new Run(newStuff))));
                            }
                            this.TbxStatus.Document.Blocks.Add(newStuffList);
                        }


                        var bugFixes = UpdaterApplication.GetAllBugfixesFromVersion(new Version(version));
                        if (bugFixes.Count > 0)
                        {
                            this.TbxStatus.Document.Blocks.Add(new Paragraph(new Run("Bugfixes:")));
                            var bugFixlist = new List();
                            foreach (var bugfix in bugFixes)
                            {
                                bugFixlist.ListItems.Add(new ListItem(new Paragraph(new Run(bugfix))));
                            }
                            this.TbxStatus.Document.Blocks.Add(bugFixlist);
                            this.TbxStatus.Document.Blocks.Add(new Paragraph(new InlineUIContainer(ruler)));
                        }
                    });
                }

                // Update the application
                this.Updater.UpdateApplication(this.UpdaterApplication, SelfUpdater.ApplicationDirectory, Sun.Core.SelfUpdating.SelfUpdater.SUN_UPDATER_ROOTURL);

                // After Update is completed, start caller Application
                Process.Start(SelfUpdater.ApplicationToStartAfterUpdate);

                // Close SelfUpdater
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    SelfUpdater.Current.Shutdown();
                });
            }
        }

        private void updaterApp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateStatus")
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    this.LblStatus.Content = this.UpdaterApplication.UpdateStatus;
                });
            }
            if (e.PropertyName == "UpdateProgress")
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    CtlProgress.IsIndeterminate = false;
                    CtlProgress.Maximum = 1;
                    CtlProgress.Minimum = 0;
                    CtlProgress.Value = this.UpdaterApplication.UpdateProgress;
                });
            }
        }

        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        public void FinishAndStartApp()
        {
            Process.Start(Sun.SelfUpdater.SelfUpdater.ApplicationToStartAfterUpdate);
            Sun.SelfUpdater.SelfUpdater.Current.Shutdown();
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, args);
        }

        #endregion
    }
}
