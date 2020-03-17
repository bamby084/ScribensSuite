using System.Windows.Input;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class UnInstallationConfirmViewModel: BaseViewModel
    {
        #region ctors
        public UnInstallationConfirmViewModel()
        {
            CancelCommand = new RelayCommand(Cancel);
            UnInstallCommand = new RelayCommand(UnInstall);
        }
        #endregion

        #region Commands
        public ICommand UnInstallCommand { get; set; }
        public  ICommand CancelCommand { get; set; }

        private void Cancel(object param)
        {
            EventManager.PublishEvent(new ExitInstallationEvent());
        }

        private void UnInstall(object param)
        {
            EventManager.PublishEvent(new InstallEvent(){Action = SetupAction.UnInstall});
        }
        #endregion
    }
}
