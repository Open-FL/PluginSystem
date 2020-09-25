using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class InstallPackageEventArgs : AddPackageEventArgs<BasePluginPointer>
    {

        public InstallPackageEventArgs(bool isNew, BasePluginPointer data, string tempDir, bool canCancel) :
            base(data, canCancel)
        {
            TempDir = tempDir;
            IsNew = isNew;
        }

        public bool IsNew { get; }

        public string TempDir { get; }

    }
}