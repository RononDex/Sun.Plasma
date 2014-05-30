using Sun.Core.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for News.xaml
    /// </summary>
    public partial class ViewNews : UserControl, IWpfCachableControl
    {
        ViewModel.ViewModelNews vm;

        public ViewNews()
        {
            InitializeComponent();
            this.vm = new ViewModel.ViewModelNews();
            this.DataContext = vm;
            vm.PropertyChanged += vm_PropertyChanged;
            LoadNews();
        }

        /// <summary>
        /// Handles clicks on hyperlinks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        /// <summary>
        /// Starts loading the news from the server asynchroniously
        /// </summary>
        private void LoadNews()
        {
            Thread loadingThread = new Thread(vm.LoadNewsFeeds);
            loadingThread.Start();
        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // When news feeds got loaded by the asynchronious thread
            if (e.PropertyName == "NewsFeeds")
            {
                this.Dispatcher.Invoke(new Action(delegate()
                {
                    CtlLoadingNews.Visibility = System.Windows.Visibility.Collapsed;
                    CtlNews.Visibility = System.Windows.Visibility.Visible;
                }));
            }
        }

        public void ReloadingFromCacheForDisply()
        {
            CtlLoadingNews.Visibility = System.Windows.Visibility.Visible;
            CtlNews.Visibility = System.Windows.Visibility.Collapsed;

            LoadNews();
        }
    }
}
