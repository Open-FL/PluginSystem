using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class AfterAddPackageEventArgs : CancelableEventArgs
    {

        public AfterAddPackageEventArgs(BasePluginPointer pointer) : base(false)
        {
            Pointer = pointer;
        }

        public BasePluginPointer Pointer { get; }

    }
}