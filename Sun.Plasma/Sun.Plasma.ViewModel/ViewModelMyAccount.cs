using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Sun.Plasma.Controller.Objects;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelMyAccount : ViewModelBase
    {
        public List<Ship> MyShips {
            get { return new List<Ship>() { new Ship() { ID = 50, ShipName = "Prometheus", ShipType = "Constellation"} }; }
            //set;
        }
    }
}
