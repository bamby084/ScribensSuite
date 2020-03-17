using System.Windows.Input;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class SelectProductViewModel: BaseViewModel
    {
        #region ctor
        public SelectProductViewModel()
        {
            InstallCommand = new RelayCommand(Install);
            CancelCommand = new RelayCommand(Cancel);
        }
        #endregion

        #region Commands
        public ICommand InstallCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private void Install(object param)
        {
            EventManager.PublishEvent(new InstallEvent(){Action = SetupAction.Install});
        }

        private void Cancel(object param)
        {
            EventManager.PublishEvent(new ExitInstallationEvent());
        }
        #endregion
    }
}
