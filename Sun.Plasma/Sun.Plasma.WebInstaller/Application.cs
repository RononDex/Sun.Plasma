using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Security;
using System.ComponentModel;

namespace Sun.Plasma.WebInstaller
{
    /// <summary>
    /// Represents an selfupdate-enabled application
    /// </summary>
    public class Application : INotifyPropertyChanged
    {
        /// <summary>
        /// Make it impossible to initialize this class from another assembly
        /// by setting the constructor to internal
        /// </summary>
        internal Application()
        {

        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, args);
        }

        #endregion

        /// <summary>
        /// Contains the changelog as a xml document
        /// </summary>
        public XDocument ChangeLog { get; set; }

        /// <summary>
        /// Name of the application
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the published application
        /// </summary>
        public Version ApplicationVersion { get; set; }

        /// <summary>
        /// Files that belong to this application and have to get downloaded
        /// </summary>
        public List<string> Files { get; set; }

        private float _updateProgress;
        /// <summary>
        /// Holds the progress of the current update operation
        /// </summary>
        public float UpdateProgress 
        { 
            get { return _updateProgress; }
            set { _updateProgress = value; OnPropertyChanged("UpdateProgress"); }
        }

        private string _updateStatus;
        public string UpdateStatus
        {
            get { return this._updateStatus; }
            set { this._updateStatus = value; OnPropertyChanged("UpdateStatus"); }
        }

        /// <summary>
        /// The root URL of the software repository
        /// </summary>
        public string WebRootUrl { get; set; }

        private string _publishingStatus;
        /// <summary>
        /// The status of the publishing
        /// </summary>
        public string PublishingStatus 
        { 
            get { return this._publishingStatus; }
            set { this._publishingStatus = value; OnPropertyChanged("PublishingStatus"); }
        }

        private float _publishingProgress;
        /// <summary>
        /// The progress of the publishing between 0 and 1
        /// </summary>
        public float PublishingProgress 
        {
            get { return this._publishingProgress; }
            set { this._publishingProgress = value; OnPropertyChanged("PublishingProgress"); }
        }

        /// <summary>
        /// Gets the changelog for the given version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public XElement GetChangelogForVersion(Version version)
        {
            if (this.ChangeLog == null)
                return null;

            var changeLogEntry = this.ChangeLog.Root.Elements("Version").Where(entry => entry.Attribute( XName.Get("Version")).Value == version.ToString()).FirstOrDefault();
            return changeLogEntry;
        }

        /// <summary>
        /// Gets a list of all bugfixes from a defined version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<string> GetAllBugfixesFromVersion(Version version)
        {
            if (this.ChangeLog == null)
                return null;

            var versionElement = GetChangelogForVersion(version);
            if (versionElement == null)
                return null;

            var bugfixElements = versionElement.Elements("Bugfix");

            var result = new List<string>();
            foreach (var bugfix in bugfixElements)
            {
                result.Add(bugfix.Value);
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all bugfixes from a defined version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public List<string> GetAllNewStuffFromVersion(Version version)
        {
            if (this.ChangeLog == null)
                return null;

            var versionElement = GetChangelogForVersion(version);
            if (versionElement == null)
                return null;

            var newElements = versionElement.Elements("New");

            var result = new List<string>();
            foreach (var newElement in newElements)
            {
                result.Add(newElement.Value);
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all registered versions inside the changelog file
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllVersionsFromChangeLog()
        {
            if (this.ChangeLog == null)
                return null;

            var result = new List<string>();
            var versions = this.ChangeLog.Root.Elements("Version").Attributes(XName.Get("Version"));
            foreach (var version in versions)
            {
                result.Add(version.Value);
            }
            return result;
        }

        /// <summary>
        /// Adds the version to the changelog
        /// </summary>
        /// <param name="version"></param>
        /// <param name="bugFixes"></param>
        /// <param name="newStuff"></param>
        public void AddVersionToChangelog(Version version, List<string> bugFixes, List<string> newStuff)
        {
            // If not changelog exists so far, setup the xml document
            if (this.ChangeLog == null)
            {
                this.ChangeLog = new XDocument();
                this.ChangeLog.Add(new XElement(XName.Get("ChangeLog")));
            }

            // Create new Version entry
            var newVersionElement = new XElement(XName.Get("Version"));
            newVersionElement.Add(new XAttribute(XName.Get("Version"), string.Format("{0}.{1}.{2}", version.MajorRevision, version.MinorRevision, version.Build)));

            // Add BugFixes
            foreach (var bugFix in bugFixes)
            {
                var element = new XElement(XName.Get("Bugfix"), bugFix);
                newVersionElement.Add(element);
            }

            // Add new stuff
            foreach (var newEntry in newStuff)
            {
                var element = new XElement(XName.Get("New"), newEntry);
                newVersionElement.Add(element);
            }

            this.ChangeLog.Root.Add(newVersionElement);
        }
    }
}