using System;
using System.IO;
using System.Net;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;

namespace PluginSystem.Updating
{
    public class WebPointerUpdateChecker : IPluginUpdateChecker
    {

        public bool CanCheck(BasePluginPointer ptr)
        {
            if (ptr.PluginOrigin == "" || !ptr.PluginOrigin.EndsWith("info.txt"))
            {
                return false;
            }

            Uri origin = ptr.PluginOriginUri;
            return IsWebPointer(origin);
        }

        public void CheckAndUpdate(
            BasePluginPointer ptr, Func<string, string, bool> updateDialog, Action<string, int, int> setStatus)
        {
            setStatus?.Invoke($"[{ptr.PluginName}] Searching Updates.", 0, 1);
            BasePluginPointer originPtr = GetPointer(ptr.PluginOrigin);

            if (ptr.PluginVersion >= originPtr.PluginVersion)
            {
                setStatus?.Invoke($"[{ptr.PluginName}] Up to date.", 1, 1);
                return;
            }

            setStatus?.Invoke($"[{ptr.PluginName}] Waiting for User Input", 1, 3);
            if (updateDialog(
                             $"Do you want to update {ptr.PluginName} : {ptr.PluginVersion} to {originPtr.PluginName} : {originPtr.PluginVersion}?",
                             $"Update: {originPtr.PluginVersion}"
                            ))
            {
                string tempFile = Path.Combine(
                                               PluginPaths.GetPluginTempDirectory(ptr),
                                               $"{originPtr.PluginName}_{originPtr.PluginVersion}_.zip"
                                              );

                setStatus?.Invoke($"[{ptr.PluginName}] Installing Update", 1, 3);

                DownloadFile(originPtr, tempFile);

                PluginManager.AddPackage(tempFile, out string name);

                File.Delete(tempFile);
            }
            else
            {
                setStatus?.Invoke($"[{ptr.PluginName}] User denied update request.", 1, 3);
            }
        }

        public static bool IsWebPointer(Uri origin)
        {
            return origin.Scheme == "http" || origin.Scheme == "https";
        }

        public static void DownloadFile(BasePluginPointer ptr, string file)
        {
            using (WebClient wc = new WebClient())
            {
                string uri = ptr.PluginOriginUri.AbsoluteUri.Remove(
                                                                    ptr.PluginOriginUri.AbsoluteUri.Length -
                                                                    "info.txt".Length,
                                                                    "info.txt".Length
                                                                   ) +
                             ptr.PluginName +
                             ".zip";
                wc.DownloadFile(uri, file);
            }
        }

        public static string DownloadFile(BasePluginPointer ptr)
        {
            string tempFile = Path.Combine(
                                           PluginPaths.SystemTempDirectory,
                                           $"{ptr.PluginName}_{ptr.PluginVersion}_.zip"
                                          );
            DownloadFile(ptr, tempFile);
            return tempFile;
        }

        public static BasePluginPointer GetPointer(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return new BasePluginPointer(wc.DownloadString(url));
            }
        }

    }
}