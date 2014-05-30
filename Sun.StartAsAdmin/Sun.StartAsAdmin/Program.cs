using System;   
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Sun.StartAsAdmin
{
    /// <summary>
    /// This application is a simple console-application, that starts the
    /// given executable with admin privileges
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // Check if there is a parameter given
            if (args.Length < 1)
            {
                Console.WriteLine("Invalid arguments!");
                Console.WriteLine("You have to call it using the following argument:");
                Console.WriteLine("Sun.StartAsAdmin.exe <PathToExecutable>");
                return;
            }

            // Make sure that the executable file exists
            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Error launching application: Could not find a part of the path {0}", args[0]);
                return;
            }

            // Starts the given application as administrator
            var startInfo = new ProcessStartInfo(args[0]);
            if (args.Length > 1)
                startInfo.Arguments = args[1];

            startInfo.Verb = "runas";
            Process.Start(startInfo);
        }
    }
}
