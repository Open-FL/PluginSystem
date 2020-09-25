using System;

namespace PluginSystem.Exceptions
{
    public class AssemblyLoadException : PluginIOException
    {

        public AssemblyLoadException(string message, string file, Exception inner) : base(message, file, inner)
        {
        }

        public AssemblyLoadException(string message, string file) : base(message, file)
        {
        }

    }
}