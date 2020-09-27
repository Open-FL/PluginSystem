using System;
using System.Collections.Generic;
using System.IO;

using PluginSystem.Core;
using PluginSystem.Exceptions;

namespace PluginSystem.Utility
{
    /// <summary>
    /// Helper Class that loads/saves a List from/to file.
    /// </summary>
    public static class ListHelper
    {

        /// <summary>
        /// Load all Lines of a File.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>The File Content</returns>
        public static string[] LoadList(string path)
        {
            try
            {
                return File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                PluginManager.SendError(new ListIOException("Could not load list", path, e));
                return new string[0];
            }
        }

        /// <summary>
        /// Saves all Lines to a File.
        /// </summary>
        /// <param name="path">Target Path</param>
        /// <param name="list">Content</param>
        public static void SaveList(string path, IEnumerable<string> list)
        {
            try
            {
                File.WriteAllLines(path, list);
            }
            catch (Exception e)
            {
                PluginManager.SendError(new ListIOException("Could not save list", path, e));
            }
        }

    }
}