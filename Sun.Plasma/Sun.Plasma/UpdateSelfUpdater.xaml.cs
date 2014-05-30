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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaction logic for UpdateSelfUpdater.xaml
    /// </summary>
    public partial class UpdateSelfUpdater : Window
    {
        FrameworkElement CtlMoveable;
        ViewModel.ViewModelUpdateSelfUpdater vm;

        public UpdateSelfUpdater()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();
            vm = new ViewModel.ViewModelUpdateSelfUpdater();
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

            // Disable Minimize and close buttons
            var BtnClose = (FrameworkElement)this.Template.FindName("BtnClose", this);
            if (BtnClose != null)
                BtnClose.Visibility = System.Windows.Visibility.Hidden;

            var BtnMinimize = (FrameworkElement)this.Template.FindName("BtnMinimize", this);
            if (BtnMinimize != null)
                BtnMinimize.Visibility = System.Windows.Visibility.Hidden;

            // Check for updates async
            // and register an event to catch its answer
            vm.LoadApplicationData();
            vm.SelfUpdaterApp.PropertyChanged += vm_PropertyChanged;
            vm.UpdateSelfUpdaterAsync();
            CtlProgress.Maximum = 1;
        }

        private void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UpdateProgress")
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate()
                {
                    CtlProgress.Value = vm.SelfUpdaterApp.UpdateProgress;
                    if (vm.SelfUpdaterApp.UpdateProgress == 1)
                    {
                        var window = new CheckForUpdates();
                        window.Show();
                        this.Close();
                    }
                });
            }
        }

        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
