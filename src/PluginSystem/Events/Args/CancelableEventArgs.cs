namespace PluginSystem.Events.Args
{
    public abstract class CancelableEventArgs
    {

        protected CancelableEventArgs(bool canCancel)
        {
            CanCancel = canCancel;
        }

        public bool Cancel { get; set; }

        public bool CanCancel { get; }

    }
}