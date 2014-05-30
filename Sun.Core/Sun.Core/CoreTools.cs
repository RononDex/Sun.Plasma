using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Core
{
    /// <summary>
    /// Static class that holds tools for the core
    /// </summary>
    public static class CoreTools
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
                    _logger = LogManager.GetLogger("Sun.Core");
                }

                return _logger;
            }
        }
    }
}
