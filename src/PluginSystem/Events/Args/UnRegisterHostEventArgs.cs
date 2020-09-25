namespace PluginSystem.Events.Args
{
    public class UnRegisterHostEventArgs : CancelableEventArgs
    {

        public UnRegisterHostEventArgs(object host, bool canCancel) : base(canCancel)
        {
            Host = host;
        }

        public object Host { get; }

    }
}