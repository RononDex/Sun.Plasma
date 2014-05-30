using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Plasma
{
    /// <summary>
    /// A static class that provides several utilities for Plasma
    /// </summary>
    internal static class PlasmaTools
    {
        private static Core.Wpf.WpfControlCache _controlsCache = new Core.Wpf.WpfControlCache();

        /// <summary>
        /// A cache for WPF controls that need to be unloaded
        /// and reloaded with keeping their state
        /// </summary>
        internal static Core.Wpf.WpfControlCache ControlsCache { get { return _controlsCache; } }

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
