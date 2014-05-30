using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net;
using System.Reflection;

namespace Sun.Plasma.WebInstaller
{
    /// <summary>
    /// This static class contains all funtions related to self updating
    /// </summary>
    public class PlasmaWebInstaller
    {
        internal const string APP_NAME = "Sun.Plasma";
        internal const string UPDATER_APPNAME = "Sun.SelfUpdater";

        /// <summary>
        /// Gets the application with the given name from the server
        /// </summary>
        /// <param name="app"></param>
        public Application GetApplication(string name, string rootUrl)
        {
            // Make sure that the rootUrl ends with a '/'
            if (!rootUrl.EndsWith("/"))
                rootUrl += "/";

            var application = new Application()
            {
                Name = name,
                Files = new List<string>(),
                ApplicationVersion = new Version(),
                WebRootUrl = rootUrl
            };

            // Create a Http client to download data
            using (var httpClient = new WebClient())
            {
                // Open filelist file on the server and store it in the object
                var versionRegex = new Regex(@"^[vV][0-9]*\.[0-9]*\.[0-9]*");

                var filesContent = new StreamReader(httpClient.OpenRead(string.Format("{0}{1}/files.txt", rootUrl, name)));
                while (!filesContent.EndOfStream)
                {
                    var line = filesContent.ReadLine();

                    // If line matches version format, parse version-number of current version
                    if (versionRegex.IsMatch(line))
                    {
                        application.ApplicationVersion = new Version(line.Substring(1));
                    }
                    else
                    {
                        application.Files.Add(line);
                    }
                }

                // Get changelog
                var changelogReader = new StreamReader(httpClient.OpenRead(string.Format("{0}{1}/changelog.xml", rootUrl, name)));
                var xmlContent = changelogReader.ReadToEnd();
                application.ChangeLog = System.Xml.Linq.XDocument.Parse(xmlContent);

                return application;
            }
        }


        /// <summary>
        /// Downloads the application files from the server and updates it local
        /// </summary>
        /// <param name="app">The application to update</param>
        /// <param name="localDirectory">The local directory path of the application</param>
        public void DownloadApplication(Application app, string localDirectory, string rootUrl)
        {
            try
            {
                // Open Http client to download files from remote server
                using (var httpClient = new WebClient())
                {
                    app.UpdateProgress = 0;
                    var downloadCount = 0;
                    // Download all files from the server
                    foreach (var file in app.Files)
                    {
                        app.UpdateStatus = string.Format("Downloading file {0}", file);
                        var localPath = Path.Combine(localDirectory, file.Replace("/", "\\"));
                        if (!File.Exists(localPath))
                        {
                            File.Delete(localPath);
                        }

                        httpClient.DownloadFile(string.Format("{2}/{0}/{1}", app.Name, file, rootUrl), localPath);
                        downloadCount++;
                        app.UpdateProgress = (float)app.Files.Count / (float)downloadCount;
                    }

                    app.UpdateStatus = "Download finished";
                }
            }
            catch { app.UpdateStatus = "Error"; throw; }
        }
    }
}
