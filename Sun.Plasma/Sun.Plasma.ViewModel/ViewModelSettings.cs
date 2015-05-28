using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sun.Plasma.ViewModel.Commands;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelSettings : ViewModelBase
    {
        public ICommand CloseCommand
        {
            get { return new CloseCommand(); }
        }
    }
}
