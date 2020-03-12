using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ScribensMSWord.Utils;

namespace ScribensMSWord.WPF.Controls
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        private ObservableCollection<Language> _languages;
        public ObservableCollection<Language> Languages
        {
            get => _languages;
            set
            {
                if (value != _languages)
                {
                    _languages = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Settings _settings;
        public Settings Settings
        {
            get => _settings;
            set
            {
                if (value != _settings)
                {
                    _settings = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Localization
        private string _backgroundCheckingHeader;
        public string BackgroundCheckingHeader
        {
            get => _backgroundCheckingHeader;
            set
            {
                if (value != _backgroundCheckingHeader)
                {
                    _backgroundCheckingHeader = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _allowBackgroundCheckingText;
        public string AllowBackgroundCheckingText
        {
            get => _allowBackgroundCheckingText;
            set
            {
                if (value != _allowBackgroundCheckingText)
                {
                    _allowBackgroundCheckingText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _checkEveryText;
        public string CheckEveryText
        {
            get => _checkEveryText;
            set
            {
                if (value != _checkEveryText)
                {
                    _checkEveryText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _intervalUnitText;
        public string IntervalUnitText
        {
            get => _intervalUnitText;
            set
            {
                if (value != _intervalUnitText)
                {
                    _intervalUnitText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _saveButtonText;
        public string SaveButtonText
        {
            get => _saveButtonText;
            set
            {
                if (value != _saveButtonText)
                {
                    _saveButtonText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _cancelButtonText;
        public string CancelButtonText
        {
            get => _cancelButtonText;
            set
            {
                if (value != _cancelButtonText)
                {
                    _cancelButtonText = value;
                    NotifyPropertyChanged();
                }
            }
        } 
        #endregion

        #region Commands
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion

        #region ctors
        public SettingsPage()
        {
            InitializeComponent();
            _languages = new ObservableCollection<Language>(Utils.Language.SupportedLanguages);
            _settings = Globals.Settings;

            BackgroundCheckingHeader = Globals.GetString("SettingsWindow.BackgroundChecking");
            AllowBackgroundCheckingText = Globals.GetString("SettingsWindow.AllowBackgroundChecking");
            CheckEveryText = Globals.GetString("SettingsWindow.CheckInterval");
            IntervalUnitText = Globals.GetString("SettingsWindow.CheckIntervalUnit");
            CancelButtonText = Globals.GetString("SettingsWindow.Cancel");
            SaveButtonText = Globals.GetString("SettingsWindow.Save");

            this.DataContext = this;
        }
        #endregion

        #region Methods
        public void Save()
        {
            _settings.Save();
        }
        #endregion
    }
}
