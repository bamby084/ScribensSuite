using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class MainViewModel: BaseViewModel,
        IEventHandler<BeginEvent>,
        IEventHandler<ExitEvent>,
        IEventHandler<CancelEvent>,
        IEventHandler<AcceptLicenseAgreementEvent>
    {
        #region Properties
        private BaseViewModel _activeViewModel;
        public BaseViewModel ActiveViewModel
        {
            get => _activeViewModel;
            set
            {
                if (value != _activeViewModel)
                {
                    _activeViewModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BootstrapperApplication Bootstrapper { get; private set; }
        #endregion

        #region ctors
        public MainViewModel(BootstrapperApplication bootstrapper)
        {
            Bootstrapper = bootstrapper;
            ActiveViewModel = new WelcomeViewModel();
        }
        #endregion

        #region Event Handlers
        public void HandleEvent(BeginEvent @event)
        {
            if (@event.FromScreen == SetupScreen.Welcome)
                ActiveViewModel = new LicenseAgreementViewModel();
            else if(@event.FromScreen == SetupScreen.ProductSelection)
                ActiveViewModel = new SetupViewModel(Bootstrapper);
        }

        public void HandleEvent(ExitEvent @event)
        {
            Bootstrapper.Engine.Quit(1602);//user exit
        }

        public void HandleEvent(AcceptLicenseAgreementEvent @event)
        {
            ActiveViewModel = new SelectProductViewModel();
        }

        public void HandleEvent(CancelEvent @event)
        {
            ((Bootstrapper)Bootstrapper).BootstrapDispatcher.InvokeShutdown();
        }
        #endregion

        #region Methods
        #endregion
    }
}
