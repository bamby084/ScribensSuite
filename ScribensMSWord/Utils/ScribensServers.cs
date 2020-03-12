using System;
using System.Collections.Generic;

namespace ScribensMSWord.Utils
{
    public static class ScribensServers
    {
        private static int _serverIndex = 0;
        private static string[] _servers = new string[] { "Scribens", "X2", "X4", "X6" };
        private static readonly Dictionary<string, string> _hostDictionary = new Dictionary<string, string>()
        {
            {"en", "https://www.scribens.com"},
            {"fr", "https://www.scribens.fr"}
        };

        static ScribensServers()
        {
            var random = new Random(Environment.TickCount);
            _serverIndex = random.Next(_servers.Length);
        }

        public static string GetHost(string language)
        {
            if (!_hostDictionary.ContainsKey(language))
                return string.Empty;

            return _hostDictionary[language];
        }

        public static string GetDefaultServerName()
        {
            return _servers[0];
        }

        public static string GetServerName()
        {
            return _servers[_serverIndex];
        }
    }
}
