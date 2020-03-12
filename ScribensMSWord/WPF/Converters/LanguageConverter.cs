using Newtonsoft.Json;
using System;
using System.Linq;
using PluginScribens_Word.Utils;

namespace PluginScribens_Word.WPF.Converters
{
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
