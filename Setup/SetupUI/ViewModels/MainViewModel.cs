using System.Windows;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class MainViewModel: BaseViewModel,
        IEventHandler<InstallEvent>,
        IEventHandler<ExitInstallationEvent>,
        IEventHandler<StopInstallationEvent>,
        IEventHandler<CompleteEvent>
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
            Bootstrapper.DetectPackageComplete += OnDetectPackageComplete;
        }
        #endregion

        #region Event Handlers
        public void HandleEvent(InstallEvent @event)
        {
            if (@event.Action == SetupAction.ShowLicenseAgreement)
                ActiveViewModel = new LicenseAgreementViewModel();
            else if (@event.Action == SetupAction.SelectProducts)
                ActiveViewModel = new SelectProductViewModel();
            else if(@event.Action == SetupAction.Install)
                ActiveViewModel = new ProgressViewModel(this.Bootstrapper, SetupAction.Install);
            else if(@event.Action == SetupAction.UnInstall)
                ActiveViewModel = new ProgressViewModel(this.Bootstrapper, SetupAction.UnInstall);
            else if (@event.Action == SetupAction.InstallComplete)
                ActiveViewModel = new CompleteViewModel(SetupAction.InstallComplete);
            else if(@event.Action == SetupAction.UnInstallComplete)
                ActiveViewModel = new CompleteViewModel(SetupAction.UnInstallComplete);
        }

        public void HandleEvent(ExitInstallationEvent @event)
        {
            Bootstrapper.Engine.Quit(1602);//user exit
        }

        public void HandleEvent(StopInstallationEvent @event)
        {
            ((Bootstrapper)Bootstrapper).BootstrapDispatcher.InvokeShutdown();
        }

        public void HandleEvent(CompleteEvent @event)
        {
            Bootstrapper.Engine.Quit(0);
        }
        #endregion

        #region Methods
        #endregion

        #region Wix Events
        private void OnDetectPackageComplete(object sender, DetectPackageCompleteEventArgs e)
        {
            if (e.PackageId == "ScribensMSOfficePlugin")
            {
                if (e.State == PackageState.Absent)
                {
                    ActiveViewModel = new WelcomeViewModel();
                }
                else if (e.State == PackageState.Present)
                {
                    ActiveViewModel = new UnInstallationConfirmViewModel();
                }
            }
        }
        #endregion
    }
}
