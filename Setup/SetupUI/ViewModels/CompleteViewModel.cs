using System.Windows.Input;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class CompleteViewModel: BaseViewModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand CloseCommand { get; set; }

        public CompleteViewModel(SetupAction action)
        {
            CloseCommand = new RelayCommand(Close);
            if (action == SetupAction.InstallComplete)
                Title = "Installation terminée";
            else if (action == SetupAction.UnInstallComplete)
                Title = "Désinstallation terminée";
        }

        private void Close(object param)
        {
            EventManager.PublishEvent(new CompleteEvent());
        }
    }
}
