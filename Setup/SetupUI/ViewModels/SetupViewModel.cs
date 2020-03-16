using System.Windows.Input;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class SetupViewModel: BaseViewModel
    {
        #region Properties
        private bool _isCancel = false;

        private int _progressPercentage;
        public int ProgressPercentage
        {
            get => _progressPercentage;
            set
            {
                if (value != _progressPercentage)
                {
                    _progressPercentage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _status;
        public string Status
        {
            get => _status;
            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyPropertyChanged();
                }
            }
        } 

        public BootstrapperApplication Bootstrapper { get; set; }
        #endregion

        #region ctors
        public SetupViewModel(BootstrapperApplication bootstrapper)
        {
            CancelCommand = new RelayCommand(Cancel);
            Bootstrapper = bootstrapper;
            Bootstrapper.ApplyComplete += OnApplyComplete;
            Bootstrapper.DetectPackageComplete += OnDetectPackageComplete;
            Bootstrapper.PlanComplete += OnPlanComplete;
            Bootstrapper.ExecuteProgress += OnExecuteProgress;
            Bootstrapper.CacheAcquireProgress += OnCacheAcquireProgress;

            Bootstrapper.Engine.Plan(LaunchAction.Install);
        }
        #endregion

        #region Methods
        
        #endregion

        #region Commands
        public  ICommand CancelCommand { get; set; }

        private void Cancel(object param)
        {
            _isCancel = true;
        }
        #endregion

        #region Bootstrapper Events
        private void OnCacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {

        }

        private void OnExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
            if (_isCancel)
            {
                e.Result = Result.Cancel;
            }
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

    }
}
