using System;
using System.IO;
using System.Windows.Input;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using SetupUI.Enums;
using SetupUI.Events;

namespace SetupUI.ViewModels
{
    public class ProgressViewModel: BaseViewModel
    {
        #region Properties
        private bool _isCancel = false;
        private SetupAction _action;

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

        public BootstrapperApplication Bootstrapper { get; set; }
        #endregion

        #region ctors
        public ProgressViewModel(BootstrapperApplication bootstrapper, SetupAction action)
        {
            CancelCommand = new RelayCommand(Cancel);
            Bootstrapper = bootstrapper;
            Bootstrapper.ApplyComplete += OnApplyComplete;
            Bootstrapper.PlanComplete += OnPlanComplete;
            Bootstrapper.ExecuteProgress += OnExecuteProgress;
            Bootstrapper.ExecuteMsiMessage += OnExecuteMsiMessage;
            Bootstrapper.ResolveSource += OnResolveSource;
            if (action == SetupAction.Install)
            {
                Title = "Progression de l'installation";
                Bootstrapper.Engine.Plan(LaunchAction.Install);
            }
            else if (action == SetupAction.UnInstall)
            {
                Title = "Progression de la désinstallation";
                Bootstrapper.Engine.Plan(LaunchAction.Uninstall);
            }

            _action = action;
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
            if(e.Status >= 0)
            {
                Bootstrapper.Engine.Apply(IntPtr.Zero);
            }
        }

        private void OnApplyComplete(object sender, ApplyCompleteEventArgs e)
        {
            ProgressPercentage = 100;

            EventManager.PublishEvent(new InstallEvent()
            {
                Action = _action == SetupAction.Install
                    ? SetupAction.InstallComplete
                    : SetupAction.UnInstallComplete
            });
        }

        private void OnExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            Status = e.Message;
        }

        private void OnResolveSource(object sender, ResolveSourceEventArgs e)
        {
            if (!File.Exists(e.LocalSource) && !string.IsNullOrEmpty(e.DownloadSource))
            {
                e.Result = Result.Download;
            }
        }
        #endregion
    }
}
