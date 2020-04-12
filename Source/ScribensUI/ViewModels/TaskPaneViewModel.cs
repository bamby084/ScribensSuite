using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using PluginScribens.Common;
using PluginScribens.Common.GrammarChecker;
using PluginScribens.UI.Messages;
using Task = System.Threading.Tasks.Task;

namespace PluginScribens.UI.ViewModels
{
    public class TaskPaneViewModel: BaseViewModel,
        IMessageHandler<LogInMessage>, 
        IMessageHandler<ResetSnapshotMessage>,
        IMessageHandler<ShowSolutionsMessage>
    {
        private BackgroundChecker _backgroundChecker;

        #region ctors
        public TaskPaneViewModel()
        {
            SolutionsViewModel = new SolutionsViewModel();
            UserInfoViewModel = new UserInfoViewModel();
        }
        #endregion

        #region Properties
        public SolutionsViewModel SolutionsViewModel { get; }

        public UserInfoViewModel UserInfoViewModel { get; }

        private bool _isBackgroundCheckerEnabled;
        public bool IsBackgroundCheckerEnabled
        {
            get => _isBackgroundCheckerEnabled;
            set
            {
                _isBackgroundCheckerEnabled = value;

                if (_backgroundChecker != null)
                    _backgroundChecker.IsEnabled = _isBackgroundCheckerEnabled;
            }
        }

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

        private IWindow _windowHost;
        public override IWindow WindowHost
        {
            get => _windowHost;
            set
            {
                if (value != _windowHost)
                {
                    _windowHost = value;
                    UserInfoViewModel.WindowHost = _windowHost;
                    SolutionsViewModel.WindowHost = _windowHost;
                    NotifyPropertyChanged();
                }
            }
        }

        private Document _associatedDocument;
        public Document AssociatedDocument
        {
            get => _associatedDocument;
            set
            {
                if (value != _associatedDocument)
                {
                    _associatedDocument = value;
                    SolutionsViewModel.AssociatedDocument = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        public void ShowSolutions()
        {
            ActiveViewModel = SolutionsViewModel;
        }

        public async Task ShowUserInfo()
        {
            ActiveViewModel = UserInfoViewModel;
            await UserInfoViewModel.AutoLogin();
        }

        public void UpdateLocalizationResources()
        {
            UserInfoViewModel.UpdateLocalizationResources();
            SolutionsViewModel.UpdateLocalizationResources();
        }

        public void StartBackgroundChecker()
        {
            _backgroundChecker = new BackgroundChecker(AssociatedDocument, new ScribensGrammarChecker());
            _backgroundChecker.OnBeforeChecking += OnBackgroundBeforeChecking;
            _backgroundChecker.OnStartChecking += OnBackgroundStartChecking;
            _backgroundChecker.OnCheckCompleted += OnBackgroundCheckCompleted;
            _backgroundChecker.OnLimCharExceeded += OnBackgroundLimCharExceeded;
            _backgroundChecker.Start();
        }

        #endregion

        #region Background Checker Events
        private void OnBackgroundBeforeChecking(object sender, ref bool cancel)
        {
            if (Globals.CurrentIdentity != null)
            {
                if (Globals.CurrentIdentity.IsExpired())
                {
                    Debug.WriteLine("Cancel checking::Account is expired");
                    Messenger.SendMessage(new ShowErrorMessage(new AccountExpiredErrorViewModel() { WindowHost = this.WindowHost }), this);
                    cancel = true;
                }
            }
        }

        private void OnBackgroundStartChecking(object sender, StartCheckingEventArgs e)
        {       
            Messenger.SendMessage(new CheckingStartsMessage(), this);
        }

        private void OnBackgroundCheckCompleted(object sender, CheckCompletedEventArgs e)
        {
            Messenger.SendMessage(new CheckingCompletedMessage()
            {
                Solutions = e.Solutions,
                ParagraphIndices = e.ParagraphIndices,
                IndPSupp = e.IndPSupp,
                DiffNbPar = e.DiffNbPar
            }, this);
        }

        private void OnBackgroundLimCharExceeded(object sender, LimCharExceededEventArgs e)
        {
            Messenger.SendMessage(new LimCharExceededMessage(), this);
        }
        #endregion

        #region Messages
        public void HandleMessage(LogInMessage message)
        {
            ActiveViewModel = UserInfoViewModel;
        }

        public void HandleMessage(ResetSnapshotMessage message)
        {
            _backgroundChecker.ResetSnapshot();
        }

        public void HandleMessage(ShowSolutionsMessage message)
        {
            if (ActiveViewModel != SolutionsViewModel)
                ShowSolutions();
        }
        #endregion
    }
}
