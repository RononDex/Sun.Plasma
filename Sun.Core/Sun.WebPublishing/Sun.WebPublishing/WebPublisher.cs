using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sun.Core.SelfUpdating;
using System.Security;
using Renci.SshNet;
using System.Reflection;
using Sun.Core.Security;
using System.IO;

namespace Sun.WebPublishing
{
    public class WebPublisher
    {
        public readonly string[] IGNORELIST_FILEEXSTENSIONS = new string[] { ".pdb", ".vshost.exe", ".manifest", ".vshost.exe.config", ".log" };

        /// <summary>
        /// Initialy uploads the application to the server
        /// </summary>
        /// <param name="app"></param>
        /// <param name="localPath">The path to the local .exe file of the application</param>
        public void InitialUpload(string appName, string localPath, List<string> bugFixes, List<string> newStuff, string ftpServer, string ftpUser, SecureString ftpPassword, string pathToAppsFolder)
        {
            var app = new Application();
            app.ApplicationVersion = new Version();
            app.Name = appName;

            PublishApplication(app, localPath, bugFixes, newStuff, new List<string>(), ftpServer, ftpUser, ftpPassword, pathToAppsFolder);
        }

        /// <summary>
        /// Publishes the application to the server
        /// </summary>
        /// <param name="app"></param>
        /// <param name="localPath">The path to the local .exe file of the application</param>
        public void PublishApplication(Application app, string localPath, List<string> bugFixes, List<string> newStuff, List<string> deletedFiles, string ftpServer, string ftpUser, SecureString ftpPassword, string pathToAppsFolder)
        {
            try
            {
                #region PRECONDITIONS

                if (!File.Exists(localPath))
                    throw new FileNotFoundException(string.Format("Could not find a part of the file '{0}'", localPath));

                #endregion

                app.UpdateProgress = 0;
                app.PublishingStatus = "Connecting to SFTP server";

                // Open SFTP connection to server
                using (var ftpClient = new SftpClient(ftpServer, ftpUser, SecureStringUtility.SecureStringToString(ftpPassword)))
                {
                    ftpClient.Connect();

                    var newVersion = AssemblyName.GetAssemblyName(localPath).Version;
                    if (newVersion <= app.ApplicationVersion)
                        throw new InvalidOperationException("You are trying to publish an older version than the one that is on the server! You have to increase the assembly version and republish!");

                    app.PublishingStatus = "Updating changelog";

                    // Create changelog entry
                    app.AddVersionToChangelog(newVersion, bugFixes, newStuff, deletedFiles);
                    ftpClient.UploadFile(this.GenerateStreamFromString(app.ChangeLog.ToString()), string.Format("{1}/{0}/changelog.xml", app.Name, pathToAppsFolder), true);

                    app.PublishingStatus = "Gathering data";

                    // Load all files that have to get uploaded
                    var localDirectory = Path.GetDirectoryName(localPath);
                    var files = GetFileListRecursive(localDirectory, new List<string>(), localDirectory);
                    app.Files = files;

                    // Create new fileIndex with new version info at first line
                    var filesIndexContent = string.Format("v{0}.{1}.{2}\r\n", newVersion.Major, newVersion.Minor, newVersion.Build);
                    foreach (var file in app.Files)
                    {
                        filesIndexContent += file + "\r\n";
                    }

                    var filePath = string.Format("{1}/{0}/files.txt", app.Name, pathToAppsFolder);
                    ftpClient.UploadFile(GenerateStreamFromString(filesIndexContent), filePath, true, null);

                    app.PublishingStatus = "Uploading files";
                    var uploadedFiles = 0;

                    // Upload all files
                    foreach (var file in app.Files)
                    {
                        app.PublishingStatus = string.Format("Downloading file {0}", file);
                        var localFilePath = Path.Combine(localDirectory, file);
                        var remotePath = string.Format("{2}/{0}/{1}", app.Name, file, pathToAppsFolder);

                        ftpClient.UploadFile(File.Open(localFilePath, FileMode.Open, FileAccess.Read), remotePath, true);
                        uploadedFiles++;
                        app.PublishingProgress = (float)app.Files.Count / (float)uploadedFiles;
                    }

                    app.PublishingStatus = "Publishing finished";
                }
            }
            catch { app.PublishingStatus = "Error"; throw; }
        }

        /// <summary>
        /// Generates a stream from a string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Gets a recursive list of all files that have to get uploaded to the server
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileList"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        private List<string> GetFileListRecursive(string path, List<string> fileList, string basePath)
        {
            var files = Array.FindAll(Directory.GetFiles(path), file => !IGNORELIST_FILEEXSTENSIONS.Any(ignore => file.EndsWith(ignore))).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                // Remove local path (make it relative)
                files[i] = files[i].Replace(basePath, string.Empty).Substring(1);
            }

            fileList.AddRange(files);

            foreach (var subDirectory in Directory.GetDirectories(path))
            {
                fileList = GetFileListRecursive(subDirectory, fileList, basePath);
            }

            return fileList;
        }
    }

     
}
