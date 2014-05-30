using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Plasma.ViewModel
{
    /// <summary>
    /// This static class contains some useful utilities & tools for the view model
    /// </summary>
    public static class ViewModelTools
    {
        private static ILog _logger;
        /// <summary>
        /// The Logger which can be used to log stuff from the core
        /// </summary>
        public static ILog Logger
        {
            get
            {
                if (_logger == null)
                {
                    XmlConfigurator.Configure();
                    _logger = LogManager.GetLogger("Sun.Plasma");
                }

                return _logger;
            }
        }
    }
}
