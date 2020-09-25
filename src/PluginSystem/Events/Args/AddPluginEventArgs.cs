using PluginSystem.Core.Pointer;

namespace PluginSystem.Events.Args
{
    public class AddPluginEventArgs : CancelableEventArgs
    {

        public AddPluginEventArgs(PluginAssemblyPointer pointerData, object plugin, bool canCancel) : base(canCancel)
        {
            PointerData = pointerData;
            Plugin = plugin;
        }

        public PluginAssemblyPointer PointerData { get; }

        public object Plugin { get; }

    }
}