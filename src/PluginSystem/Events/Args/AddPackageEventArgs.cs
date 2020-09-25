namespace PluginSystem.Events.Args
{
    public class AddPackageEventArgs<T> : CancelableEventArgs
    {

        public AddPackageEventArgs(T data, bool canCancel) : base(canCancel)
        {
            Data = data;
        }

        public T Data { get; }

    }
}