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
    /// Interaktionslogik für ViewAccount.xaml
    /// </summary>
    public partial class ViewAccount : UserControl
    {
        private ViewModel.ViewModelMyAccount vm;

        public ViewAccount()
        {
            InitializeComponent();
            this.vm = new ViewModel.ViewModelMyAccount();
            this.DataContext = vm;
        }
    }
}
