using System;

namespace PluginSystem.Exceptions
{
    public class ListIOException : PluginIOException
    {

        public ListIOException(string message, string file, Exception inner) : base(message, file, inner)
        {
        }

        public ListIOException(string message, string file) : base(message, file)
        {
        }

    }
}