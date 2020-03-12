using ScribensMSWord.ExtensionMethods;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ScribensMSWord.Attributes;
using ScribensMSWord.Checkers.IdentityChecker;
using ScribensMSWord.Enums;
using ScribensMSWord.Utils;
using ScribensMSWord.WPF.Messages;

namespace ScribensMSWord.WPF.ViewModels
{
    public class UserInfoViewModel: BaseViewModel,
        IMessageHandler<LogInCompletedMessage>, 
        IMessageHandler<LogOutMessage>
    {
        public UserInfoViewModel()
        {
            InitCommands();
            UpdateLocalizationResources();
        }

        #region Properties
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("CanConnect");
                }
            }
        }

        public bool CanConnect => !_userName.IsNull() && !_password.IsNull();

        private Visibility _loadingIndicatorVisibility = Visibility.Collapsed;
        public Visibility LoadingIndicatorVisibility
        {
            get => _loadingIndicatorVisibility;
            set
            {
                if (value != _loadingIndicatorVisibility)
                {
                    _loadingIndicatorVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility _loginSectionVisibility = Visibility.Visible;
        public Visibility LoginSectionVisibility
        {
            get => _loginSectionVisibility;
            set
            {
                if (value != _loginSectionVisibility)
                {
                    _loginSectionVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Visibility _userInfoSectionVisibility = Visibility.Collapsed;
        public Visibility UserInfoSectionVisibility
        {
            get => _userInfoSectionVisibility;
            set
            {
                if (value != _userInfoSectionVisibility)
                {
                    _userInfoSectionVisibility = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Identity _identity;
        public Identity Identity
        {
            get => _identity;
            set
            {
                if (value != _identity)
                {
                    _identity = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (value != _password)
                {
                    _password = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("CanConnect");
                }
            }
        }
        #endregion

        #region Methods
        public void UpdateLocalizationResources()
        {
            LoginLabel = Globals.GetString("UserInfoPane.Login");
            UserNameLabel = Globals.GetString("UserInfoPane.UserName");
            PasswordLabel = Globals.GetString("UserInfoPane.Password");
            ConnectButtonLabel = Globals.GetString("UserInfoPane.Connect");
            ClearButtonLabel = Globals.GetString("UserInfoPane.Clear");
            EmailLabel = Globals.GetString("UserInfoPane.Email");
            SubscriptionLabel = Globals.GetString("UserInfoPane.Subscription");
            ExpiredDateLabel = Globals.GetString("UserInfoPane.ExpiredDate");
            BecomePremiumLabel = Globals.GetString("UserInfoPane.BecomePremium");
            MyAccountLabel = Globals.GetString("UserInfoPane.MyAccount");
            SignOutLabel = Globals.GetString("UserInfoPane.SignOut");

            NotifyPropertyChanged(nameof(Status));
            NotifyPropertyChanged(nameof(Identity));
        }

        public void InitCommands()
        {
            ConnectCommand = new RelayCommand(Connect);
            SignOutCommand = new RelayCommand(SignOut);
        }

        public async Task AutoLogin()
        {
            if (Globals.CurrentIdentity != null)
            {
                Identity = Globals.CurrentIdentity;
                LoginSectionVisibility = Visibility.Collapsed;
                UserInfoSectionVisibility = Visibility.Visible;
                LoadingIndicatorVisibility = Visibility.Collapsed;

                return;
            }

            try
            {
                var loginInfo = LoginInfo.Load();
                if (loginInfo == null)
                {
                    LoadingIndicatorVisibility = Visibility.Collapsed;
                    return;
                }

                UserName = loginInfo.Username;
                await Login(loginInfo.Username, loginInfo.Password, false);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                LoadingIndicatorVisibility = Visibility.Collapsed;
            }
        }

        private async Task Login(string username, string password, bool saveIdentity)
        {
            LoadingIndicatorVisibility = Visibility.Visible;
            Status = null;

            try
            {
                IIdentityChecker identityChecker = new ScribensIdentityChecker();
                var identity = await identityChecker.CheckIdentityAsync(username, password, Globals.Settings.Language.Abbreviation);
                LoadingIndicatorVisibility = Visibility.Collapsed;

                if (identity.IsValid())
                {
                    LoginSectionVisibility = Visibility.Collapsed;
                    UserInfoSectionVisibility = Visibility.Visible;
                    Identity = identity;
                    Globals.CurrentIdentity = identity;
                    Messenger.BroadCastMessage(new LogInCompletedMessage(), this);

                    if (identity.Status == IdentityStatus.True)
                    {
                        Messenger.SendMessage(new ResetSnapshotMessage(), this);
                        Globals.ThisAddIn.StartSessionTimer();
                    }

                    if (saveIdentity)
                    {
                        LoginInfo.Save(username, password);
                    }

                    return;
                }

                var resourceMessageAttribute = identity.Status.GetAttribute<ResourceMessageAttribute>();
                Status = resourceMessageAttribute.Message;
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                LoadingIndicatorVisibility = Visibility.Collapsed;
                Status = "UserInfoPane.Message.LoginUnknownError";
            }
        }

        private void SignOut()
        {
            Status = null;
            LoginSectionVisibility = Visibility.Visible;
            UserInfoSectionVisibility = Visibility.Collapsed;
            UserName = null;
            Password = null;
        }

        private void Refresh()
        {
            if (Globals.CurrentIdentity == null)
            {
                SignOut();
            }
            else
            {
                Identity = Globals.CurrentIdentity;
                LoginSectionVisibility = Visibility.Collapsed;
                UserInfoSectionVisibility = Visibility.Visible;
            }
        }
        #endregion

        #region Localization

        private string _loginLabel;
        public string LoginLabel
        {
            get => _loginLabel;
            set
            {
                if (value != _loginLabel)
                {
                    _loginLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _userNameLabel;
        public string UserNameLabel
        {
            get => _userNameLabel;
            set
            {
                if (value != _userNameLabel)
                {
                    _userNameLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _passwordLabel;
        public string PasswordLabel
        {
            get => _passwordLabel;
            set
            {
                if (value != _passwordLabel)
                {
                    _passwordLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _connectButtonLabel;
        public string ConnectButtonLabel
        {
            get => _connectButtonLabel;
            set
            {
                if (value != _connectButtonLabel)
                {
                    _connectButtonLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _clearButtonLabel;
        public string ClearButtonLabel
        {
            get => _clearButtonLabel;
            set
            {
                if (value != _clearButtonLabel)
                {
                    _clearButtonLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _emailLabel;
        public string EmailLabel
        {
            get => _emailLabel;
            set
            {
                if (value != _emailLabel)
                {
                    _emailLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _subscriptionLabel;
        public string SubscriptionLabel
        {
            get => _subscriptionLabel;
            set
            {
                if (value != _subscriptionLabel)
                {
                    _subscriptionLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _expiredDateLabel;
        public string ExpiredDateLabel
        {
            get => _expiredDateLabel;
            set
            {
                if (value != _expiredDateLabel)
                {
                    _expiredDateLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _becomePremiumLabel;
        public string BecomePremiumLabel
        {
            get => _becomePremiumLabel;
            set
            {
                if (value != _becomePremiumLabel)
                {
                    _becomePremiumLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _myAccountLabel;
        public string MyAccountLabel
        {
            get => _myAccountLabel;
            set
            {
                if (value != _myAccountLabel)
                {
                    _myAccountLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _signOutLabel;
        public string SignOutLabel
        {
            get => _signOutLabel;
            set
            {
                if (value != _signOutLabel)
                {
                    _signOutLabel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Commands
        public ICommand ConnectCommand { get; set; }
        public async void Connect(object param)
        {
            await Login(_userName, Password, true);
        }

        public ICommand SignOutCommand { get; set; }
        public void SignOut(object param)
        {
            Globals.CurrentIdentity = null;
            SignOut();
            LoginInfo.Delete();
            Globals.ThisAddIn.StopSessionTimer();
            Messenger.BroadCastMessage(new LogOutMessage(), this);
        }
        #endregion

        #region Message Handler
        public void HandleMessage(LogInCompletedMessage message)
        {
            Refresh();
        }

        public void HandleMessage(LogOutMessage message)
        {
            Refresh();
        }
        #endregion
    }
}
