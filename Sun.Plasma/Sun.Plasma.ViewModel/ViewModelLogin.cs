using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Security;
using System.IO;
using Sun.Core.Security;

namespace Sun.Plasma.ViewModel
{
    public class ViewModelLogin : ViewModelBase
    {
        readonly string CREDENTIAL_FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SUNLogin.blob");


        public ViewModelLogin()
        {
            if (File.Exists(CREDENTIAL_FILE))
            {                
                SecureStorage.RestorePerUserCredentials(out _userName, out _password, CREDENTIAL_FILE);
                this.RememberMe = true;
            }
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

        public bool Login(SecureString password)
        {
            this.Password = password;
            if (RememberMe && !string.IsNullOrEmpty(this.UserName) && this.Password.Length > 0)
            {
                SecureStorage.StorePerUserCredentials(this.UserName, this.Password, CREDENTIAL_FILE);
            }
            else
            {
                if (File.Exists(CREDENTIAL_FILE))
                    File.Delete(CREDENTIAL_FILE);
            }

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
