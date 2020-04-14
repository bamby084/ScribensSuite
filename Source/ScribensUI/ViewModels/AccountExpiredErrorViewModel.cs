using System.Diagnostics;
using System.Windows.Input;
using PluginScribens.Common;

namespace PluginScribens.UI.ViewModels
{
    public class AccountExpiredErrorViewModel: ErrorViewModel
    {
        #region ctors
        public AccountExpiredErrorViewModel()
        {
            InitializeCommands();
        }
        #endregion

        #region Properties
        public override string ErrorMessage => "TaskPane.AccountExpired";
        public string SubscribeMessage { get; } = "TaskPane.Subscribe";
        public string BuyPremiumMessage { get; } = "TaskPane.BuySubscription";
        #endregion

        #region Methods
        public void InitializeCommands()
        {
            BecomePremiumCommand = new RelayCommand(BecomePremium);
        }
        #endregion

        #region Commands
        public ICommand BecomePremiumCommand { get; set; }
        private void BecomePremium(object param)
        {
            string url = "https://www.scribens.fr/?key=VersionPremium";
            Process.Start(new ProcessStartInfo(url));
        }
        #endregion

    }
}
