using System;

namespace PluginSystem.Exceptions
{
    public abstract class PluginException : Exception
    {

        protected PluginException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PluginException(string message) : base(message)
        {
        }

        public virtual string ExceptionName => GetType().Name;

    }
}