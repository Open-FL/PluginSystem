namespace PluginSystem.Events.Args
{
    public class CleanTempFolderEventArgs : CancelableEventArgs
    {

        public CleanTempFolderEventArgs() : base(true)
        {
        }

    }
}