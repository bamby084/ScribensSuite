using System.Windows.Input;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class LicenseAgreementViewModel: BaseViewModel
    {
        #region ctors
        public LicenseAgreementViewModel()
        {
            AcceptCommand = new RelayCommand(OnAccept);
            CancelCommand = new RelayCommand(OnCancel);
        }
        #endregion

        #region Commands
        public  ICommand AcceptCommand { get; set; }
        public  ICommand CancelCommand { get; set; }

        private void OnAccept(object param)
        {
            EventManager.PublishEvent(new AcceptLicenseAgreementEvent());
        }

        private void OnCancel(object param)
        {
            EventManager.PublishEvent(new ExitEvent());
        }
        #endregion
    }
}
