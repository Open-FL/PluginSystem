using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class ActivatePackageEventArgs : CancelableEventArgs
    {

        public ActivatePackageEventArgs(BasePluginPointer packagePtr, bool canCancel) : base(canCancel)
        {
            PackagePointer = packagePtr;
        }

        public BasePluginPointer PackagePointer { get; }

    }
}