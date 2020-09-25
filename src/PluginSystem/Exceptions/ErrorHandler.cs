using System;
using System.IO;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.FileSystem;

namespace PluginSystem.Exceptions
{
    public static class ErrorHandler
    {

        private static string[] IgnoredExceptions;

        private static string ErrorConfig =>
            Path.Combine(
                         PluginPaths.InternalConfigurationDirectory,
                         "ignored-exceptions" + StaticData.PluginListExtension
                        );

        public static event Action<Exception> OnIgnoredException;

        internal static void Initialize()
        {
            if (!File.Exists(ErrorConfig))
            {
                File.WriteAllText(ErrorConfig, "");
            }

            IgnoredExceptions = File.ReadAllLines(ErrorConfig);

            PluginManager.RegisterErrorHandler(OnError);
        }

        private static void OnError(PluginException ex)
        {
            if (IgnoredExceptions.Contains(ex.ExceptionName))
            {
                OnIgnoredException?.Invoke(ex);
            }
            else
            {
                throw ex;
            }
        }

    }
}