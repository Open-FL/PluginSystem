using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;

namespace PluginSystem.FileSystem.PackageData
{
    public static class PackageDataManager
    {

        /// <summary>
        /// Installed Package Data Formats
        /// </summary>
        private static readonly List<APackageDataFormat> PackerMap = new List<APackageDataFormat>();

        /// <summary>
        /// Count of Installed Package Data Formats
        /// </summary>
        public static int FormatCount => PackerMap.Count;


        /// <summary>
        /// Adds a Package Data Format to the system
        /// </summary>
        /// <param name="formatProvider">The format to be added.</param>
        public static void AddPackerFormat(APackageDataFormat formatProvider)
        {
            if (PackerMap.All(x => x.GetType() != formatProvider.GetType()))
            {
                PluginManager.SendLog("Adding Package Data Format: " + formatProvider.GetType().Name);
                PackerMap.Add(formatProvider);
            }
        }


        /// <summary>
        /// Removes a Package Data Format from the system
        /// </summary>
        /// <param name="formatProvider">The format to be removed.</param>
        public static void RemovePackerFormat(APackageDataFormat formatProvider)
        {
            if (PackerMap.Contains(formatProvider))
            {
                PluginManager.SendLog("Removing Package Data Format: " + formatProvider.GetType().Name);
                PackerMap.Remove(formatProvider);
            }
        }


        /// <summary>
        /// Checks if the Format can Load the Specified File
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>True if this Format can load the Type of File</returns>
        public static bool CanLoad(string folder)
        {
            return PackerMap.Any(x => x.CanLoad(folder));
        }


        /// <summary>
        /// Loads the BasePluginPointer from a Directory
        /// </summary>
        /// <param name="folder">The Folder containing the Unpacked Files</param>
        /// <returns>The Files that were Built</returns>
        public static BasePluginPointer LoadData(string folder)
        {
            PluginManager.SendLog("Loading Data from Folder: " + Path.GetFileName(folder));
            APackageDataFormat format = PackerMap.FirstOrDefault(x => x.CanLoad(folder));
            PluginManager.SendLog("Selected Format: " + format.GetType().Name);
            return format?.LoadData(folder);
        }

        /// <summary>
        /// Installs the Specified folder contents into the Directories of the specified pointer
        /// </summary>
        /// <param name="ptr">The Pointer</param>
        /// <param name="folder">The Folder with contents</param>
        public static void Install(BasePluginPointer ptr, string folder)
        {
            APackageDataFormat format = PackerMap.FirstOrDefault(x => x.CanLoad(folder));
            PluginManager.SendLog(
                                  $"Installing Package {ptr.PluginName} from {Path.GetFileName(folder)} with format {format.GetType().Name}"
                                 );
            format?.Install(ptr, folder);
        }

        /// <summary>
        /// Returns the Format at Index i
        /// </summary>
        /// <param name="i">Index of the Format.</param>
        /// <returns>The Format located at the specfied index i</returns>
        public static APackageDataFormat GetFormatAt(int i)
        {
            return PackerMap[i];
        }

    }
}