using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class MainViewModel: BaseViewModel,
        IEventHandler<StartSetupEvent>,
        IEventHandler<CancelSetupEvent>,
        IEventHandler<UserAcceptLicenseAgreementEvent>
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

        public MainViewModel(BootstrapperApplication bootstrapper)
        {
            Bootstrapper = bootstrapper;
            Bootstrapper.ApplyComplete += OnApplyComplete;
            Bootstrapper.DetectPackageComplete += OnDetectPackageComplete;
            Bootstrapper.PlanComplete += OnPlanComplete;
            Bootstrapper.ExecuteProgress += OnExecuteProgress;
            Bootstrapper.CacheAcquireProgress += OnCacheAcquireProgress;

            ActiveViewModel = new WelcomeViewModel();
        }

        #region Bootstrapper Events
        private void OnCacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            
        }

        private void OnExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            
        }

        private void OnPlanComplete(object sender, PlanCompleteEventArgs e)
        {
            
        }

        private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
        }

        private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            
        }
        #endregion

        #region Event Handlers
        public void HandleEvent(StartSetupEvent @event)
        {
            ActiveViewModel = new LicenseAgreementViewModel();
        }

        public void HandleEvent(CancelSetupEvent @event)
        {
            Bootstrapper.Engine.Quit(1602);//user exit
        }

        public void HandleEvent(UserAcceptLicenseAgreementEvent @event)
        {
            ActiveViewModel = new SetupViewModel();
        }
        #endregion
    }
}
