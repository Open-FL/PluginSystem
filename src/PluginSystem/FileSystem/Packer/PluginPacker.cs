using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core;

namespace PluginSystem.FileSystem.Packer
{
    /// <summary>
    /// Packer Class Providing all Package Formats
    /// </summary>
    public static class PluginPacker
    {

        /// <summary>
        /// Installed Packer Formats
        /// </summary>
        private static readonly List<APluginPackerFormat> PackerMap = new List<APluginPackerFormat>();

        /// <summary>
        /// Count of Installed Packer Formats
        /// </summary>
        public static int FormatCount => PackerMap.Count;

        /// <summary>
        /// Adds a Packer Format to the system
        /// </summary>
        /// <param name="formatProvider">The format to be added.</param>
        public static void AddPackerFormat(APluginPackerFormat formatProvider)
        {
            if (PackerMap.All(x => x.GetType() != formatProvider.GetType()))
            {
                PluginManager.SendLog($"Adding Plugin Packer Format: {formatProvider.GetType().Name}");
                PackerMap.Add(formatProvider);
            }
        }

        /// <summary>
        /// Removes a Packer Format from the system
        /// </summary>
        /// <param name="formatProvider">The format to be removed.</param>
        public static void RemovePackerFormat(APluginPackerFormat formatProvider)
        {
            if (PackerMap.Contains(formatProvider))
            {
                PluginManager.SendLog($"Removing Plugin Packer Format: {formatProvider.GetType().Name}");
                PackerMap.Remove(formatProvider);
            }
        }

        /// <summary>
        /// Checks if the System can Load the Specified File
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>True if a Format can load the Type of File</returns>
        public static bool CanLoad(string file)
        {
            return PackerMap.Any(x => x.CanLoad(file));
        }

        /// <summary>
        /// Unpacks a File into an Output Directory
        /// </summary>
        /// <param name="file">File to Unpack</param>
        /// <param name="outputDirectory">Output Directory</param>
        public static void Unpack(string file, string outputDirectory)
        {
            PluginManager.SendLog("Loading Data from File: " + Path.GetFileName(file));
            APluginPackerFormat format = PackerMap.FirstOrDefault(x => x.CanLoad(file));
            PluginManager.SendLog($"Selected Format: {format.GetType().Name}");
            PluginManager.SendLog($"Output Directory: {Path.GetFileName(outputDirectory)}");
            format?.Unpack(file, outputDirectory);
        }

        /// <summary>
        /// Returns the Format at Index I
        /// </summary>
        /// <param name="i">Index of the Format.</param>
        /// <returns>The Format located at the specfied index i</returns>
        public static APluginPackerFormat GetFormatAt(int i)
        {
            return PackerMap[i];
        }

    }
}