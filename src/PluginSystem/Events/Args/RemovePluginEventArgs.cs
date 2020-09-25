namespace PluginSystem.Events.Args
{
    public class RemovePluginEventArgs : CancelableEventArgs
    {

        public RemovePluginEventArgs(object host, object plugin, bool canCancel) : base(canCancel)
        {
            Host = host;
            Plugin = plugin;
        }

        public object Host { get; }

        public object Plugin { get; }

    }
}