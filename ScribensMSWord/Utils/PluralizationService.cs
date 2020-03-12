using PluginScribens_Word.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace PluginScribens_Word.Utils
{
    public class PluralizationService
    {
        private static Dictionary<string, IPluralizationService> _pluralizationServices = new Dictionary<string, IPluralizationService>()
        {
            {"fr-FR", new FrenchPluralizationService() }
        };
        private IPluralizationService _internalPluralizationService;

        private PluralizationService(CultureInfo culture)
        {
            if (!_pluralizationServices.ContainsKey(culture.Name))
                throw new NotSupportedException();

            _internalPluralizationService = _pluralizationServices[culture.Name];
        }

        public static PluralizationService Create(CultureInfo culture)
        {
            return new PluralizationService(culture);
        }

        public string Pluralize(string source)
        {
            if (source.IsNull())
                return null;

            string[] words = Regex.Split(source, @"\s+");
            return string.Join(" ", words.Select(w => _internalPluralizationService.Pluralize(w)));
        }

        private interface IPluralizationService {
            string Pluralize(string word);
        }

        private class FrenchPluralizationService : IPluralizationService
        {
            public string Pluralize(string word)
            {
                if (string.IsNullOrWhiteSpace(word) || word.EndsWith("s") || word.EndsWith("x") || word.EndsWith("z"))
                    return word;

                if (word == "un")
                    return "des";

                if (word.EndsWith("au"))
                    return $"{word}x";

                if (word.EndsWith("al"))
                    return word.Substring(0, word.Length - 2) + "aux";

                return $"{word}s";
            }
        }
    }
}
