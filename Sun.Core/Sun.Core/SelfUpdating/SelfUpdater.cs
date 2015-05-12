using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sun.Core.Ftp;
using System.Security;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Sun.Core.Security;
using System.Net;
using System.Reflection;

namespace Sun.Core.SelfUpdating
{
    /// <summary>
    /// This static class contains all funtions related to self updating
    /// </summary>
    public class SelfUpdater
    {
        public const string UPDATER_APPNAME = "Sun.SelfUpdater";
        public const string UPDATER_FILE = "Sun.SelfUpdater.exe";
        public const string SUN_UPDATER_ROOTURL = "http://systemsunitednavy.com/apps";

        /// <summary>
        /// Gets the application with the given name from the server
        /// </summary>
        /// <param name="app"></param>
        public Application GetApplication(string name, string rootUrl)
        {
            CoreTools.Logger.DebugFormat("Getting online application info for \"{0}\" from {1}", name, rootUrl);

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
                        var versions = line.Substring(1).Split(new string[] {"."}, StringSplitOptions.RemoveEmptyEntries);
                        application.ApplicationVersion = new Version(Convert.ToInt32(versions[0]),
                                                                    Convert.ToInt32(versions[1]),
                                                                    Convert.ToInt32(versions[2]),
                                                                    0);
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

                CoreTools.Logger.DebugFormat("Online application information for \"{0}\" loaded:", name);
                CoreTools.Logger.DebugFormat(" - Online version: {0}", application.ApplicationVersion);

                return application;
            }
        }

        /// <summary>
        /// Downloads the application files from the server and updates it local
        /// </summary>
        /// <param name="app">The application to update</param>
        /// <param name="localPath">The local path of the application to the executable file</param>
        public void UpdateApplication(Application app, string localPath, string rootUrl)
        {
            CoreTools.Logger.InfoFormat("Updating application \"{0}\" from {1} in {2}", app.Name, rootUrl, localPath);

            try
            {
                var localDirectory = Path.GetDirectoryName(localPath);
                var oldVersion = AssemblyName.GetAssemblyName(localPath).Version;

                // Open Http client to download files from remote server
                using (var httpClient = new WebClient())
                {
                    app.UpdateProgress = 0;
                    var downloadCount = 0;

                    // Download all files from the server
                    foreach (var file in app.Files)
                    {
                        app.UpdateStatus = string.Format("Downloading file {0}", file);
                        var localFilePath = Path.Combine(localDirectory, file.Replace("/", "\\"));

                        httpClient.DownloadFile(string.Format("{2}/{0}/{1}", app.Name, file, rootUrl), localFilePath);
                        downloadCount++;
                        app.UpdateProgress = (float)downloadCount / (float)app.Files.Count;
                        CoreTools.Logger.DebugFormat("Updated file {0}", localFilePath);
                    }

                    app.UpdateStatus = "Deleting unnecessary files";

                    // Delete all deletedFiles from all newer versions
                    foreach (var version in app.GetAllVersionsFromChangeLog())
                    {
                        if (new Version(version) > oldVersion)
                        {
                            // Delete all deletedFiles
                            foreach (var deletedFile in app.GetAllDeletedFilesForVersion(new Version(version)))
                            {
                                var localFilePath = Path.Combine(localDirectory, deletedFile.Replace("/", "\\"));
                                if (File.Exists(localFilePath))
                                {
                                    File.Delete(localFilePath);
                                    CoreTools.Logger.DebugFormat("Deleted file {0}", localFilePath);
                                }
                            }
                        }
                    }

                    app.UpdateStatus = "Update finished";
                }
            }
            catch (Exception ex)
            {
                app.UpdateStatus = "Error";
                CoreTools.Logger.ErrorFormat("Error updating application \"{0}\": {1}", app.Name, ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Checks if there is a new version of the given application available
        /// </summary>
        /// <param name="rootUrl"></param>
        /// <param name="applicationName"></param>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public bool CheckForUpdates(string applicationName, string rootUrl, Version currentVersion)
        {
            #if DEBUG
            return false;
            #endif

            CoreTools.Logger.DebugFormat("Checking for updates for application \"{0}\"", applicationName);
            var onlineApp = this.GetApplication(applicationName, rootUrl);

            var updateAvailable = onlineApp.ApplicationVersion > currentVersion;

            if (updateAvailable)
                CoreTools.Logger.InfoFormat("Found new version for \"{0}\": {1}. Current-Version: {2}", applicationName, onlineApp.ApplicationVersion, currentVersion);
            else
                CoreTools.Logger.InfoFormat("No new version found for \"{0}\". Current-Version: {1}", applicationName, currentVersion);

            return updateAvailable;
        }
    }
}
