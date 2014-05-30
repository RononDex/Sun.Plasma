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
using Sun.Core.Security;

namespace Sun.Plasma
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        FrameworkElement CtlMoveable;
        ViewModel.ViewModelLogin vm;

        public LoginWindow()
        {
            this.Loaded += OnLoaded;
            InitializeComponent();
            vm = new ViewModel.ViewModelLogin();
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
            this.TbxUserName.Focus();

            if (this.vm.RememberMe)
                TbxPassword.Password = SecureStringUtility.SecureStringToString(vm.Password);
        }

        public void OnMoveableClick(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnLoginClick(object sender, RoutedEventArgs e)
        {
            if (vm.Login(TbxPassword.SecurePassword))
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }
    }
}
