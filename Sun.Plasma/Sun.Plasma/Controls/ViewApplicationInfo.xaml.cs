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

namespace Sun.Plasma.Controls
{
    /// <summary>
    /// Interaction logic for ViewApplicationInfo.xaml
    /// </summary>
    public partial class ViewApplicationInfo : UserControl
    {
        public ViewApplicationInfo()
        {
            InitializeComponent();

            this.DataContext = new ViewModel.ViewModelApplicationIInfo();
        }
    }
}
