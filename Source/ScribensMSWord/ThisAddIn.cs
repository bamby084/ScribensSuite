using Microsoft.Office.Interop.Word;
using System.Net;
using log4net.Repository.Hierarchy;
using log4net;
using log4net.Layout;
using log4net.Appender;
using System.IO;
using log4net.Core;
using Task = System.Threading.Tasks.Task;
using Timer = System.Timers.Timer;
using System.Timers;
using PluginScribens.Common;
using PluginScribens.Common.Enums;
using PluginScribens.Common.ExtensionMethods;
using PluginScribens.Common.IdentityChecker;
using PluginScribens.Common.SessionChecker;
using PluginScribens.UI.Hosts;
using PluginScribens_Word.Properties;

namespace PluginScribens_Word
{
    public partial class ThisAddIn
    {
        #region Properties
        private Timer _timer;

        private AddinRibbon _ribbon;
        public AddinRibbon Ribbon => _ribbon; 
        #endregion

        #region VSTO Event Handlers
        private async void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            EnableTLS();
            RegisterEvents();
            ConfigureLogger();
            SetResourcesCulture();
            await AutoLogin();
        }

        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            _ribbon = new AddinRibbon();
            return _ribbon;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            UnRegisterEvents();
        }
        #endregion

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        #region Methods

        public void RefreshRibbon()
        {
            SetResourcesCulture();
            Ribbon.Invalidate();
        }

        public void StartSessionTimer()
        {
            if(_timer == null)
            {
                _timer = new Timer();
                _timer.Interval = Plugin.Settings.SessionActiveTimerInterval * 1000;
                _timer.Elapsed += OnSessionActiveTimerElapsed;
            }

            //timer already started?
            if (_timer.Enabled)
                return;

            _timer.Start();
        }

        public void StopSessionTimer()
        {
            if(_timer != null)
            {
                _timer.Stop();
            }
        }

        private void EnableTLS()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        private void RegisterEvents()
        {
            Application.WindowSelectionChange += OnSelectionChanged;
            Application.DocumentOpen += OnDocumentOpen;
            Application.DocumentBeforeClose += OnDocumentBeforeClose;
            Application.DocumentBeforeSave += OnDocumentBeforeSave;
            Application.SubscribeAfterSave(OnDocumentAfterSave);
            ((ApplicationEvents4_Event)Application).NewDocument += OnNewDocument;
        }

        private void UnRegisterEvents()
        {
            Application.WindowSelectionChange -= OnSelectionChanged;
            Application.DocumentOpen -= OnDocumentOpen;
            Application.DocumentBeforeClose -= OnDocumentBeforeClose;
            Application.DocumentBeforeSave -= OnDocumentBeforeSave;
            ((ApplicationEvents4_Event)Application).NewDocument -= OnNewDocument;
        }

        private void ConfigureLogger()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%newline%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender();
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.AppendToFile = true;
            roller.File = Path.Combine(Utils.Settings.GetAppDataFolder(), "log.txt");
            roller.Layout = patternLayout;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "10MB";
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.StaticLogFileName = true;
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = Level.Info;
            hierarchy.Configured = true;
        }

        private void SetResourcesCulture()
        {
            Strings.Culture = Plugin.CurrentCulture;
        }

        private async Task AutoLogin()
        {
            var loginInfo = LoginInfo.Load();
            if (loginInfo == null)
                return;

            IIdentityChecker idChecker = new ScribensIdentityChecker();
            var identity = await idChecker.CheckIdentityAsync(loginInfo.Username, loginInfo.Password, Plugin.Settings.Language.Abbreviation);
            
            if(identity.IsValid())
            {
                Plugin.CurrentIdentity = identity;
                if (identity.Status == IdentityStatus.True)
                    StartSessionTimer();
            }
        }
        #endregion

        #region App Event Handlers
        private void OnNewDocument(Document document)
        {
            var applicationWindow = Application.Windows.GetWindowByDocument(document);
            if (applicationWindow != null)
                Plugin.Windows.AddNewWindow(applicationWindow.Hwnd);
        }

        private void OnDocumentOpen(Document document)
        {
            var applicationWindow = Application.Windows.GetWindowByDocument(document);
            if (applicationWindow != null)
                Plugin.Windows.AddNewWindow(applicationWindow.Hwnd);
        }

        private void OnDocumentBeforeClose(Document document, ref bool cancel)
        {
            var applicationWindow = Application.Windows.GetWindowByDocument(document);
            if (applicationWindow == null)
                return;

            IWindow window = Plugin.Windows.GetWindow(applicationWindow.Hwnd);
            if (window != null)
                window.RemoveTaskPane();
        }

        private void OnDocumentBeforeSave(Document document, ref bool saveAsUI, ref bool cancel)
        {
            var taskPane = document.GetTaskPane();
            if (taskPane == null)
                return;

            var taskPaneHost = (TaskPaneHost)taskPane.Control;
            taskPaneHost.TaskPane.Context.SolutionsViewModel.ClearHighLights();
        }

        private void OnDocumentAfterSave(Document document)
        {
            if (document == null)
                return;

            var taskPane = document.GetTaskPane();
            if (taskPane == null || !taskPane.Visible)
                return;

            var taskPaneHost = (TaskPaneHost)taskPane.Control;
            taskPaneHost.TaskPane.Context.SolutionsViewModel.ShowHighLights();
        }

        private void OnSelectionChanged(Selection selection)
        {
            var taskPane = selection.Document.GetTaskPane();
            if (taskPane == null || !taskPane.Visible)
                return;

            var taskPaneHost = (TaskPaneHost)taskPane.Control;
            taskPaneHost.TaskPane.Context.SolutionsViewModel.HandleMistakenWordSelection(selection);
        }
        #endregion

        #region Events
        private void OnSessionActiveTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(async () => await Session.NotifyScribensServer());
        }
        #endregion
    }
}
