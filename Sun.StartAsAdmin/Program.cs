using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.StartAsAdmin
{
    /// <summary>
    /// This simple and small program starts a given executable with elevated rights
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Error: Invalid Parameters");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Error: Could not find executable {0}", args[0]);
                return;
            }

            string param = string.Empty;
            for (var i = 1; i < args.Length; i++)
            {
                param += " " + args[i];
            }

            Process.Start(args[0], param);
        }
    }
}
