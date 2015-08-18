using Sun.Plasma.ViewModel;
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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Diagnostics;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FrameworkElement CtlMoveable;        

        public MainWindow()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();

            this.DataContext = new Sun.Plasma.ViewModel.ViewModelMain() { SettingsWindow = new Settings()};
            Application.Current.MainWindow = this;

            // Show news feed by default
            ShowNewsFeed_Click(null, null);
        }

        public void OnLoaded(object sender, EventArgs e)
        {
            // Enable moving the window when clicking on the control CtlMoveable 
            this.CtlMoveable = (FrameworkElement)this.Template.FindName("CtlMoveable", this);
            if (null != this.CtlMoveable)
            {
                this.CtlMoveable.MouseLeftButtonDown += new MouseButtonEventHandler(OnMoveableClick);
            }

            // Make this window resizable
            ResizableWindow.MakeWindowResizable(this);
        }

      
        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            // Make window moveable when clicking on it
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // When Window gets hidden, then show balloon info on tray icon
            if ((bool)e.OldValue == true
                && (bool)e.NewValue == false)
            {
                PlasmaTrayIcon.NotifyIcon.ShowBalloonTip(2000, "S.U.N. Plasma",
                    "The application now runs in the background. You can doubleclick this icon at any time to bring the window back up",
                    System.Windows.Forms.ToolTipIcon.None);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #region Switch Main Control Logic

        /// <summary>
        /// Switches the content of the main control
        /// </summary>
        /// <param name="cacheControlName"></param>
        private void ChangeContent(string cacheControlName)
        {
            if (PlasmaTools.ControlsCache[cacheControlName] != null)
            {
                PnlMainContent.Content = null;

                // Call reload function on control if available
                if (PlasmaTools.ControlsCache[cacheControlName] is Core.Wpf.IWpfCachableControl)
                    ((Core.Wpf.IWpfCachableControl)PlasmaTools.ControlsCache[cacheControlName]).ReloadingFromCacheForDisply();

                PnlMainContent.Content = PlasmaTools.ControlsCache[cacheControlName];
                PnlMainContent.InvalidateVisual();
            }
        }

        private void ShowInfoControl_Click(object sender, RoutedEventArgs e)
        {
            string controlName = "InfoControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewApplicationInfo();

            ChangeContent(controlName);
        }


        private void ShowNewsFeed_Click(object sender, RoutedEventArgs e)
        {
            string controlName = "NewsFeedControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewNews();

            ChangeContent(controlName);
        }

        private void ShowFleetResources_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.systemsunitednavy.com/forums/service_resources/");
        }


        private void ShowMissionManagement_Click(object sender, RoutedEventArgs e)
        {
            string controlName = "MissionManagementControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewMissionManagement();

            ChangeContent(controlName);
        }

        private void ShowSquadron_Click(object sender, RoutedEventArgs e)
        {
            string controlName = "SquadronControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewSquadron();

            ChangeContent(controlName);
        }

        private void ShowLogistics_Click(object sender, RoutedEventArgs e)
        {
            string controlName = "LogisticsControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewLogistics();

            ChangeContent(controlName);
        }

        private void ShowAccount_Click(object sender, RoutedEventArgs e)
        {
             string controlName = "AccountControl";

            if (PlasmaTools.ControlsCache[controlName] == null)
                PlasmaTools.ControlsCache[controlName] = new Controls.ViewAccount();

            ChangeContent(controlName);
        }

        #endregion

        #region UI Helpers
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            double change = e.Delta;
            double currentPosition = scrollViewer.VerticalOffset;

            scrollViewer.ScrollToVerticalOffset(currentPosition - change);
        }

        #endregion
    }
}
