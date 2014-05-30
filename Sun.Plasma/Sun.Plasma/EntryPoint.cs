using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sun.Plasma
{
    /// <summary>
    /// Contains the custom entryPoint for this application.
    /// This entry point ensures that only one instance of this application runs
    /// </summary>
    class EntryPoint
    {
        // Unique name to identify this application
        private const string UniqueApplicationName = "Sun.Plasma";

        /// <summary>
        /// Entry point
        /// </summary>
        [STAThread]
        public static void Main()
        {
            // Ensures, that this application runs only one single instance
            if (SingleInstance<PlasmaApp>.InitializeAsFirstInstance(UniqueApplicationName))
            {
                var application = new PlasmaApp();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<PlasmaApp>.Cleanup();
            }
        }
    }
}
