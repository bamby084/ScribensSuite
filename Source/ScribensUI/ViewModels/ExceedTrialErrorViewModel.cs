using System.Diagnostics;
using System.Windows.Input;
using PluginScribens.UI.Messages;

namespace PluginScribens.UI.ViewModels
{
    public class ExceedTrialErrorViewModel: ErrorViewModel
    {
        #region ctors
        public ExceedTrialErrorViewModel()
        {
            InitializeCommands();
        }
        #endregion

        #region Properties
        public override string ErrorMessage { get; } = "TaskPane.ExceedTrial";
        public string SubscribeMessage { get; } = "TaskPane.Subscribe";
        public string BecomePremiumMessage { get; } = "UserInfoPane.BecomePremium";
        public string SignInMessage { get; } = "UserInfoPane.Login";
        #endregion

        #region Methods
        public void InitializeCommands()
        {
            BecomePremiumCommand = new RelayCommand(BecomePremium);
            LoginCommand = new RelayCommand(Login);
        }
        #endregion

        #region Commands
        public ICommand BecomePremiumCommand { get; set; }
        private void BecomePremium(object param)
        {
            string url = "https://www.scribens.fr/?key=VersionPremium";
            Process.Start(new ProcessStartInfo(url));
        }

        public ICommand LoginCommand { get; set; }
        private void Login(object param)
        {
            Messenger.SendMessage(new ResetSnapshotMessage(), this);
            Messenger.SendMessage(new LogInMessage(), this);
        }
        #endregion
    }
}
