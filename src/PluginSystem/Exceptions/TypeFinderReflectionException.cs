using System;

namespace PluginSystem.Exceptions
{
    public class TypeFinderReflectionException : PluginException
    {

        public TypeFinderReflectionException(string message, Exception inner) : base(message, inner)
        {
        }

        public TypeFinderReflectionException(string message) : base(message)
        {
        }

    }
}