using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        FrameworkElement CtlMoveable;

        public Settings()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();

            this.DataContext = new Sun.Plasma.ViewModel.ViewModelSettings();
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
    }
}
