using System;
using System.IO;

using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;

namespace PluginSystem.Updating
{
    public class FileEqualityUpdateChecker : IPluginUpdateChecker
    {

        public bool CanCheck(BasePluginPointer ptr)
        {
            if (ptr.PluginOrigin == "" || !ptr.PluginOrigin.EndsWith(".dll")) return false;
            Uri origin = ptr.PluginOriginUri;
            return origin.Scheme == "file" && File.Exists(PluginPaths.GetPluginAssemblyFile(ptr)) && File.Exists(ptr.PluginOrigin);
        }

        public void CheckAndUpdate(BasePluginPointer ptr, Func<string, string, bool> updateDialog)
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