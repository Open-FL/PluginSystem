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
            if (ptr.PluginOrigin == "" || !ptr.PluginOrigin.EndsWith("info.txt")) return false;
            Uri origin = ptr.PluginOriginUri;
            return origin.Scheme == "http" || origin.Scheme == "https";
        }

        public void CheckAndUpdate(BasePluginPointer ptr, Func<string, string, bool> updateDialog, Action<string, int, int> setStatus)
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
                string tempFile = Path.Combine(PluginPaths.GetPluginTempDirectory(ptr), $"{originPtr.PluginName}_{originPtr.PluginVersion}_.zip");

                setStatus?.Invoke($"[{ptr.PluginName}] Installing Update", 1, 3);

                DownloadFile(originPtr, tempFile);

                PluginManager.AddPackage(tempFile, out string name);
            }
            else
            {
                setStatus?.Invoke($"[{ptr.PluginName}] User denied update request.", 1, 3);
            }
        }

        private void DownloadFile(BasePluginPointer ptr, string file)
        {
            using (WebClient wc = new WebClient())
            {
                string uri = ptr.PluginOriginUri.AbsoluteUri.Remove(
                                                                    ptr.PluginOriginUri.AbsoluteUri.Length -
                                                                    "info.txt".Length,
                                                                    "info.txt".Length
                                                                   ) + ptr.PluginName + ".zip";
                wc.DownloadFile(uri, file);
            }
        }

        private BasePluginPointer GetPointer(string url)
        {
            using (WebClient wc = new WebClient())
            {
                return new BasePluginPointer(wc.DownloadString(url));
            }
        }

    }

    public class FileEqualityUpdateChecker : IPluginUpdateChecker
    {

        public bool CanCheck(BasePluginPointer ptr)
        {
            if (ptr.PluginOrigin == "" || !ptr.PluginOrigin.EndsWith(".dll")) return false;
            Uri origin = ptr.PluginOriginUri;
            return origin.Scheme == "file" && File.Exists(PluginPaths.GetPluginAssemblyFile(ptr)) && File.Exists(ptr.PluginOrigin);
        }

        public void CheckAndUpdate(BasePluginPointer ptr, Func<string, string, bool> updateDialog, Action<string, int, int> setStatus)
        {
            byte[] a = File.ReadAllBytes(PluginPaths.GetPluginAssemblyFile(ptr));
            byte[] b = File.ReadAllBytes(ptr.PluginOrigin);
            bool ret = AreEqual(a, b);
            if (ret) return;

            if (!updateDialog(
                              $"The file '{ptr.PluginOrigin}' is a different version. Do you want to Update?",
                              "Update: " + ptr.PluginName
                             ))
                return;

            File.Copy(ptr.PluginOrigin, PluginPaths.GetPluginAssemblyFile(ptr), true);
        }

        private bool AreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }

            return true;
        }

    }
}