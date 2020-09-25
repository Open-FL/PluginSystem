using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class DeactivatePackageEventArgs : CancelableEventArgs
    {

        public DeactivatePackageEventArgs(BasePluginPointer ptr, bool canCancel) : base(canCancel)
        {
            Pointer = ptr;
        }

        public BasePluginPointer Pointer { get; }

    }
}