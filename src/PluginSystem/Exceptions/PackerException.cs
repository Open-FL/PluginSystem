using System;

namespace PluginSystem.Exceptions
{
    public class PackerException : PluginIOException
    {

        public PackerException(string message, string file, Exception inner) : base(message, file, inner)
        {
        }

        public PackerException(string message, string file) : base(message, file)
        {
        }

    }
}