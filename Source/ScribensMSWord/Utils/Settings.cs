using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using PluginScribens_Word.WPF.Converters;

namespace PluginScribens_Word.Utils
{
    public class Settings: INotifyPropertyChanged
    {
        #region Properties
        private Language _language = Language.Default;

        [JsonConverter(typeof(LanguageConverter))]
        public Language Language
        {
            get => _language;
            set
            {
                if (value != _language)
                {
                    _language = value;
                    NotifyPropertyChanged();
                }
            }
        }
       
        private int _backgroundCheckingInterval = 2;//in seconds
        public int BackgroundCheckingInterval
        {
            get => _backgroundCheckingInterval;
            set
            {
                if (value != _backgroundCheckingInterval)
                {
                    _backgroundCheckingInterval = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _allowBackgroundChecking = true;
        public bool AllowBackgroundChecking
        {
            get => _allowBackgroundChecking;
            set
            {
                if (value != _allowBackgroundChecking)
                {
                    _allowBackgroundChecking = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _correctionHightLight = "wdRed";
        [JsonIgnore]
        public string CorrectionHighLight
        {
            get => _correctionHightLight;
            set
            {
                if (value != _correctionHightLight)
                {
                    _correctionHightLight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _otherSolutionsHighLight = "wdYellow";
        [JsonIgnore]
        public string OtherSolutionsHighLight
        {
            get => _otherSolutionsHighLight;
            set
            {
                if (value != _otherSolutionsHighLight)
                {
                    _otherSolutionsHighLight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _underlineStyle = "wdUnderlineWavy";
        [JsonIgnore]
        public string UnderlineStyle
        {
            get => _underlineStyle;
            set
            {
                if (value != _underlineStyle)
                {
                    _underlineStyle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string _underlineColor = "#FF0000";
        [JsonIgnore]
        public string UnderlineColor
        {
            get => _underlineColor;
            set
            {
                if (value != _underlineColor)
                {
                    _underlineColor = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public int LimitedCharacters { get; } = 1000000;

        private int _sessionActiveTimerInterval = 10;//in seconds
        public int SessionActiveTimerInterval
        {
            get => _sessionActiveTimerInterval;
            set
            {
                if (value != _sessionActiveTimerInterval)
                {
                    _sessionActiveTimerInterval = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        public void Load()
        {
            try
            {
                string settingsFilePath = GetSettingsFilePath();
                if (!File.Exists(settingsFilePath))
                    return;

                using (var textReader = new StreamReader(settingsFilePath))
                {
                    var settings = JsonConvert.DeserializeObject<Settings>(textReader.ReadToEnd());
                    this.Language = settings.Language;
                    this.AllowBackgroundChecking = settings.AllowBackgroundChecking;
                    this.BackgroundCheckingInterval = settings.BackgroundCheckingInterval;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Save()
        {
            try
            {
                string appDataPath = GetAppDataFolder();
                if (!Directory.Exists(appDataPath))
                    Directory.CreateDirectory(appDataPath);

                string settingsFilePath = GetSettingsFilePath();
                string settings = JsonConvert.SerializeObject(this);
                File.WriteAllText(settingsFilePath, settings);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public static string GetAppDataFolder()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, "Scribens");
        }

        public string GetSettingsFilePath()
        {
            string appDataPath = GetAppDataFolder();
            return Path.Combine(appDataPath, "settings.json");
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
