using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security;

namespace Sun.Core.Ftp
{
    /// <summary>
    /// FTP client class (no support for SFTP)
    /// For SFTP use the Renci.SshNet.SftpClient instead
    /// </summary>
    public class FtpClient
    {
        /// <summary>
        /// A list of all directories that should be ignored
        /// </summary>
        readonly string[] DIRECTORY_INGORE_LIST = new string[] { ".", ".." };

        /// <summary>
        /// The size of the buffer used for binary read / write operations
        /// </summary>
        const int BUFFER_SIZE = 2048;

        /// <summary>
        /// The host this FtpClient conntecs to
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// The credentials that are used to authenticate on the ftp-server
        /// </summary>
        public NetworkCredential Credenntials { get; private set; }

        /// <summary>
        /// The reusable ftpRequest object for ftp operations on the server
        /// </summary>
        private FtpWebRequest FtpRequest { get; set; }

        /// <summary>
        /// Initializes the ftp-client
        /// </summary>
        /// <param name="host">Name or IP-Adress of ftp server</param>
        /// <param name="user">Username, empty if no authentification needed</param>
        /// <param name="password">Password, empty if no authentification needed</param>
        public FtpClient(string host, string user, SecureString password)
        {
            #region PRECONDITION

            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("Host cannot be NULL!");

            #endregion

            this.Host = string.Format("ftp://{0}", host);

            // Make sure that the Host Url has a leading '/'
            if (!this.Host.EndsWith("/"))
                this.Host += '/';

            // When credentials set, store them for further use
            if (!string.IsNullOrEmpty(user))
                Credenntials = new NetworkCredential(user, password);
        }

        /// <summary>
        /// Makes the FtpRequest object ready for a new request
        /// </summary>
        /// <param name="requestUrl"></param>
        private void SetupFtpRequest(string requestUrl)
        {
            // Set up the FtpRequest object
            this.FtpRequest = (FtpWebRequest)FtpWebRequest.Create(requestUrl);
            this.FtpRequest.Credentials = this.Credenntials;
            this.FtpRequest.KeepAlive = true;
            this.FtpRequest.UsePassive = true;
            this.FtpRequest.ConnectionGroupName = Host;
        }

        /// <summary>
        /// Downloads the given file from the remote server and stores it in a local file
        /// </summary>
        /// <param name="remotePath">Path on the remote server</param>
        /// <param name="localPath">Path of local file</param>
        public void DownloadFile(string remotePath, string localPath)
        {
            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.UseBinary = true;
            this.FtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            // Get response from server
            var response = this.FtpRequest.GetResponse();
            using (var responseStream = response.GetResponseStream())
            {
                // Open local file as a stream
                using (var localStream = new FileStream(localPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // Read recieved file binary
                    var buffer = new byte[BUFFER_SIZE];
                    var bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                    while (bytesRead > 0)
                    {
                        localStream.Write(buffer, 0, bytesRead);
                        bytesRead = responseStream.Read(buffer, 0, BUFFER_SIZE);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the string content of a file on the remote server
        /// </summary>
        /// <param name="remotePath">Path to the file on the remote server</param>
        /// <returns></returns>
        public string GetFileContent(string remotePath)
        {
            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.UseBinary = true;
            this.FtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            // Get response from server
            var response = this.FtpRequest.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());

            return reader.ReadToEnd();
        }

        /// <summary>
        /// Uplaods the given File
        /// </summary>
        /// <param name="remotePath">Path (with filename) on server where file should be uploaded to</param>
        /// <param name="localPath">Path of local file to upload</param>
        public void UploadFile(string remotePath, string localPath)
        {
            #region PRECONDITION

            if (!File.Exists(localPath))
                throw new FileNotFoundException(string.Format("Couldn't find the file {0}", localPath));

            #endregion

            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", remotePath, localPath));
            this.FtpRequest.UseBinary = true;
            this.FtpRequest.Method = WebRequestMethods.Ftp.UploadFile;

            // Open fileStream on Ftp-Server
            var requestStream = this.FtpRequest.GetRequestStream();

            // Write file-content to server
            var localStream = new FileStream(localPath, FileMode.Open, FileAccess.Read);
            var buffer = new byte[BUFFER_SIZE];
            var bytesSent = localStream.Read(buffer, 0, BUFFER_SIZE);
            while (bytesSent != 0)
            {
                requestStream.Write(buffer, 0, bytesSent);
                bytesSent = localStream.Read(buffer, 0, BUFFER_SIZE);
            }
        }

        /// <summary>
        /// Deletes the file under the given path on the ftp-server
        /// </summary>
        /// <param name="remotePath"></param>
        public void DeleteFile(string remotePath)
        {
            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            
            // Execute delete command
            var response = this.FtpRequest.GetResponse();

            // Clean up
            response.Close();
            response = null;
        }

        /// <summary>
        /// Creates a directory at the given path on the remote server
        /// </summary>
        /// <param name="remotePath"></param>
        public void CreateDirectory(string remotePath)
        {
            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;

            // Execute MakeDirectory command
            var response = this.FtpRequest.GetResponse();
            response.Close();
        }

        /// <summary>
        /// Lists all files / directories in a given directory on the server
        /// </summary>
        /// <param name="remotePath">The path to the directory on the server</param>
        /// <returns></returns>
        public List<string> GetDirectoryContents(string remotePath)
        {
            // Remove '/' at the beginning
            if (remotePath.StartsWith("/"))
                remotePath = remotePath.Substring(1);

            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.Method = WebRequestMethods.Ftp.ListDirectory;

            // Get answer from ftp-server
            var response = this.FtpRequest.GetResponse();

            // Read the answer from the server
            var reader = new StreamReader(response.GetResponseStream());
            var res = new List<string>();

            while (!reader.EndOfStream)
            {
                string directory = reader.ReadLine();
                if (!DIRECTORY_INGORE_LIST.Contains(directory)
                    && !DIRECTORY_INGORE_LIST.Any(s => directory.EndsWith(string.Format("/{0}", s))))
                    res.Add(directory.Replace(remotePath, string.Empty));
            }

            return res;
        }

        /// <summary>
        /// Lists all files / directories with details in a given directory on the server
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public List<string> GetDirectoryContentsDetails(string remotePath)
        {
            // Create Request
            this.SetupFtpRequest(string.Format("{0}{1}", this.Host, remotePath));
            this.FtpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            // Get answer from ftp-server
            var response = this.FtpRequest.GetResponse();

            // Read the answer from the server
            var reader = new StreamReader(response.GetResponseStream());
            var res = new List<string>();

            while (!reader.EndOfStream)
            {
                string directory = reader.ReadLine();
                if (!DIRECTORY_INGORE_LIST.Contains(directory)
                    && !DIRECTORY_INGORE_LIST.Any(s => directory.EndsWith(string.Format("/{0}", s))))
                    res.Add(directory);
            }

            return res;
        }

        /// <summary>
        /// Gets a list of all subdirectories from the given directory on the server
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public List<string> GetSubDirectories(string remotePath)
        {
            var res = new List<string>();

            // Load the content of the given directory from the server
            var listWithDetails = GetDirectoryContentsDetails(remotePath);

            foreach (var contentDetail in listWithDetails)
            {
                // The detail list is a formatted string that contains information
                // about each sub-object of the directory. Therefor we have to split it by spaces
                // in order to get the individual metadata for a sub-object
                var splitted = contentDetail.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                // The first metadata info contains the permissions
                // The first charactar of the permission string is a 'd' when
                // it is a directory
                if (splitted[0].ToLower().StartsWith("d"))
                {
                    // The last metadata info is the name of the sub-object
                    var directoryName = splitted[splitted.Length - 1];
                    if (!DIRECTORY_INGORE_LIST.Contains(directoryName)
                            && !DIRECTORY_INGORE_LIST.Any(s => directoryName.EndsWith(string.Format("/{0}", s))))
                    res.Add(directoryName);
                }
            }

            return res;
        }

        /// <summary>
        /// Gets a list of all files in the given directory on the server
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public List<string> GetSubFiles(string remotePath)
        {
            var res = new List<string>();

            // Load the content of the given directory from the server
            var listWithDetails = GetDirectoryContentsDetails(remotePath);

            foreach (var contentDetail in listWithDetails)
            {
                // The detail list is a formatted string that contains information
                // about each sub-object of the directory. Therefor we have to split it by spaces
                // in order to get the individual metadata for a sub-object
                var splitted = contentDetail.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // The first metadata info contains the permissions
                // The first charactar of the permission string is a '-' when
                // it is a file
                if (splitted[0].ToLower().StartsWith("-"))
                {
                    // The last metadata info is the name of the sub-object
                    var fileName = splitted[splitted.Length - 1];
                    res.Add(fileName);
                }
            }

            return res;
        }
    }
}
