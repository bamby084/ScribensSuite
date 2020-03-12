using log4net;
using System;

namespace PluginScribens_Word.Utils
{
    public static class Logger
    {
        private static readonly ILog _logger = LogManager.GetLogger("ScribensWord");

        public static void Error(Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }

        public static void Warning(string message)
        {
            _logger.Warn(message);
        }
    }
}
