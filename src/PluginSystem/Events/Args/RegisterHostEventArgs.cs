namespace PluginSystem.Events.Args
{
    public class RegisterHostEventArgs : CancelableEventArgs
    {

        public RegisterHostEventArgs(object host, bool canCancel) : base(canCancel)
        {
            Host = host;
        }

        public object Host { get; }

    }
}