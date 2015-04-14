using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sun.Core.SelfUpdating;

namespace Sun.WebPublishing
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var publisher = new WebPublisher();


            var selfUpdater = new SelfUpdater();
            var app = selfUpdater.GetApplication("Sun.Plasma", SelfUpdater.SUN_UPDATER_ROOTURL);

            publisher.PublishApplication(
                // App to publish
                app, 
                // Local exe path
                @"C:\Users\TinoMaik\Source\Repos\Sun.Plasma\Sun.Plasma\Sun.Plasma\bin\Release\Sun.Plasma.exe",
                 // Bugfixes
                new List<string>(),
                // New stuff
                new List<string>() { 
                    "Updated UI with new Graphics: new Close button, new minimize button, new big button.", 
                    "News Feed displays max 10 items now",
                    "Some minor code changes"
                },
                // Files to delete
                new List<string>(),
                // FTP Server
                "systemsunitednavy.com",
                // User
                "RononDex",
                // Private key file
                @"B:\Atlantis-LAB1\dev\Plasma\SUN_PrivateKey_Converted.ppk",
                //Passphrase to encrypt private file
                "W0RKINSTAL",
                // Folder path
                "/apps");
        }
    }
}
