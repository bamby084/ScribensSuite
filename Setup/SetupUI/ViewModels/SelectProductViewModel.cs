using System.Windows.Input;
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
            EventManager.PublishEvent(new BeginEvent(){FromScreen = SetupScreen.ProductSelection});
        }

        private void Cancel(object param)
        {
            EventManager.PublishEvent(new ExitEvent());
        }
        #endregion
    }
}
