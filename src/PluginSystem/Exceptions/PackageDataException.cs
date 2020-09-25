using System;
using System.IO;

namespace PluginSystem.Exceptions
{
    public class PackageDataException : PluginIOException
    {

        public readonly string File;

        public PackageDataException(string message, string file, Exception inner) : base(message, file, inner)
        {
            File = file;
        }

        public PackageDataException(string message, string file) : base(message, file)
        {
            File = file;
        }

        public override string ToString()
        {
            return $"{Message} < On File: {Path.GetFileName(File)}({File})";
        }

    }
}