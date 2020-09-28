namespace PluginSystem.StartupActions
{
    public abstract class StartupAction
    {

        public abstract string ActionName { get; }

        public abstract void RunAction(string[] parameter);

    }
}