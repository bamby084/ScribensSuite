using System.Windows.Input;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class WelcomeViewModel: BaseViewModel
    {
        public WelcomeViewModel()
        {
            StartCommand = new RelayCommand(OnStart);    
        }

        public ICommand StartCommand { get; set; }

        private void OnStart(object param)
        {
            EventManager.PublishEvent(new InstallEvent() {Action = SetupAction.ShowLicenseAgreement});
        }
    }
}
