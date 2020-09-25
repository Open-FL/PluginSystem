using System;
using System.IO;

namespace PluginSystem.Exceptions
{
    public abstract class PluginIOException : PluginException
    {

        public readonly string File;

        protected PluginIOException(string message, string file, Exception inner) : base(message, inner)
        {
            File = file;
        }

        protected PluginIOException(string message, string file) : base(message)
        {
            File = file;
        }

        public override string ToString()
        {
            return $"{Message} < On File: {Path.GetFileName(File)}({File})";
        }

    }
}