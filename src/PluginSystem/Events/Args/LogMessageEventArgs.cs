namespace PluginSystem.Events.Args
{
    public class LogMessageEventArgs
    {

        public LogMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }

        public static implicit operator string(LogMessageEventArgs args)
        {
            return args.Message;
        }

        public static implicit operator LogMessageEventArgs(string message)
        {
            return new LogMessageEventArgs(message);
        }

    }
}