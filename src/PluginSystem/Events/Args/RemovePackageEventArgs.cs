using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class RemovePackageEventArgs : CancelableEventArgs
    {

        public RemovePackageEventArgs(BasePluginPointer pointer, bool keepArchive, bool canCancel) : base(canCancel)
        {
            KeepArchive = keepArchive;
            PointerData = pointer;
        }

        public BasePluginPointer PointerData { get; }

        public bool KeepArchive { get; set; }

    }
}