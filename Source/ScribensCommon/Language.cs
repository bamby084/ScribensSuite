using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PluginScribens.Common
{
    public class Language: IEquatable<Language>
    {
        #region Properties
        [JsonIgnore]
        public string DisplayNameResource { get; set; }

        [JsonIgnore]
        public string DisplayName => Globals.GetString(DisplayNameResource);

        public string Abbreviation { get; set; }

        public string Culture { get; set; }

        public  bool IsDefault { get; set; }

        private ImageSource _icon;
        [JsonIgnore]
        public ImageSource Icon => _icon ?? (_icon = new BitmapImage(new Uri(IconUri)));

        [JsonIgnore]
        public string IconUri { get; set; }

        private static List<Language> _supportedLanguages;
        public static IList<Language> SupportedLanguages =>
            _supportedLanguages ?? (_supportedLanguages = new List<Language>()
            {
                new Language()
                {
                    DisplayNameResource = "Language.French",
                    Abbreviation = "fr",
                    Culture = "fr-FR",
                    IsDefault = true,
                    IconUri = "pack://application:,,,/PluginScribens_Word;component/Resources/french-flag.png",
                },
                new Language()
                {
                    DisplayNameResource = "Language.English",
                    Abbreviation = "en",
                    Culture = "en-US",
                    IconUri = "pack://application:,,,/PluginScribens_Word;component/Resources/england-flag.png",
                },
            });

        public static Language Default => SupportedLanguages.FirstOrDefault(language => language.IsDefault);
        #endregion

        #region Methods
        public static Language Find(string abbreviation)
        {
            return SupportedLanguages.First(language => language.Abbreviation == abbreviation);
        }
        #endregion

        #region IEquatable
        public bool Equals(Language other)
        {
            return this.Abbreviation == other.Abbreviation;
        }
        #endregion
    }

    public class LanguageConverter : JsonConverter<Language>
    {
        public override Language ReadJson(JsonReader reader, Type objectType, Language existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            try
            {
                var language = Language.SupportedLanguages.FirstOrDefault(l => l.Abbreviation == reader.Value.ToString());
                return language;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Language.Default;
            }
        }

        public override void WriteJson(JsonWriter writer, Language value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Abbreviation);
        }
    }
}
