using System;
using System.IO;

namespace PluginSystem.Exceptions
{
    public class PackageDataException : PluginIOException
    {

        public PackageDataException(string message, string file, Exception inner) : base(message, file, inner)
        {
        }

        public PackageDataException(string message, string file) : base(message, file)
        {
        }

        public override string ToString()
        {
            return $"{Message} < On File: {Path.GetFileName(File)}({File})";
        }

    }
}