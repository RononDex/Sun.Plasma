using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using System.Security;
using System.IO;
using Sun.Core;
using Sun.Core.Security;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelLogin : ViewModelBase
    {
        private readonly string CREDENTIAL_FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SystemsUnitedNavy", "SUNLogin.blob");
        private const string CREDENTIAL_FILE_KEYNAME = "SecureCredentialsStorageEntropy";


        public ViewModelLogin()
        {
            // Check if the user has stored his credentials using "Rememeber me"
            if (File.Exists(CREDENTIAL_FILE))
            {
                SecureStorage.RestorePerUserCredentials(out _userName, out _password, CREDENTIAL_FILE, CREDENTIAL_FILE_KEYNAME);
                this.RememberMe = true;
            }

            LaunchOnStartup = ApplicationTools.IsAppRegisteredToLaunchOnStartUp("Sun.Plasma");
        }

        public ICommand CloseCommand
        {
            get { return new Commands.CloseCommand(); }
        }

        public ICommand MinimizeCommand
        {
            get { return new Commands.MinimizeCommand(); }
        }

        private string _userName;
        public string UserName 
        {
            get { return this._userName; }
            set { this._userName = value; OnPropertyChanged("UserName"); }
        }

        private SecureString _password;
        public SecureString Password 
        {
            get { return this._password; }
            set { this._password = value; OnPropertyChanged("Password"); }
        }

        private bool _rememberMe;
        public bool RememberMe 
        {
            get { return this._rememberMe; }
            set { this._rememberMe = value; OnPropertyChanged("RememberMe"); }
        }

        private bool _launchOnStartup;
        public bool LaunchOnStartup
        {
            get { return _launchOnStartup; }
            set { _launchOnStartup = value; OnPropertyChanged("LaunchOnStartup"); }
        }

        private string _errorMsg = string.Empty;
        /// <summary>
        /// The error message that gets displayed on the screen
        /// </summary>
        public string ErrorMsg 
        {
            get { return _errorMsg; }
            set
            {
                this._errorMsg = value;
                OnPropertyChanged("ErrorMsg");
            }
        }

        /// <summary>
        /// Do the login validation and log in the user if successfull
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Login(SecureString password)
        {
            this.Password = password;
            if (RememberMe && !string.IsNullOrEmpty(this.UserName) && this.Password.Length > 0)
            {
                SecureStorage.StorePerUserCredentials(this.UserName, this.Password, CREDENTIAL_FILE, CREDENTIAL_FILE_KEYNAME);
            }
            else
            {
                if (File.Exists(CREDENTIAL_FILE))
                    File.Delete(CREDENTIAL_FILE);
            }

            ApplicationTools.SetAppRegisteredToLaunchOnStartup("Sun.Plasma", LaunchOnStartup, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sun.Plasma.exe"));

            // TODO: Validate User Credentials
            if (UserName == "test" 
                && Sun.Core.Security.SecureStringUtility.SecureStringToString(password) == "test")
            {
                return true;
            }           

            this.ErrorMsg = "Invalid username or password provided. Please try again.";
            return false;
        }
    }
}
