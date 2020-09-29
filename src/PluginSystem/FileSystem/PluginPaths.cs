using System;
using System.IO;
using System.Reflection;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;

namespace PluginSystem.FileSystem
{
    /// <summary>
    ///     Class used to Query File Paths for Plugins
    /// </summary>
    public static class PluginPaths
    {

        //Plugin Initial Loading Files
        /// <summary>
        ///     The Internal System Config Path set by The PackageManager.Initialize Function
        /// </summary>
        public static string InternalSystemConfigPath { get; internal set; }

        /// <summary>
        ///     The Plugin Path set by The PackageManager.Initialize Function
        /// </summary>
        public static string PluginDirectory { get; internal set; }

        /// <summary>
        ///     Returns the Full Path to the Plugin List File
        /// </summary>
        public static string PluginListFile =>
            Path.Combine(
                         InternalSystemConfigPath,
                         StaticData.ConfigFolder,
                         StaticData.PluginListName + StaticData.PluginListExtension
                        );

        /// <summary>
        ///     Returns the Full Path to the Global Plugin List File
        /// </summary>
        public static string GlobalPluginListFile =>
            Path.Combine(
                         InternalSystemConfigPath,
                         StaticData.ConfigFolder,
                         StaticData.GlobalPluginListName + StaticData.PluginListExtension
                        );

        /// <summary>
        ///     Returns the Full Path to the Init Plugin List File
        /// </summary>
        public static string InitPluginListFile =>
            Path.Combine(
                         InternalSystemConfigPath,
                         StaticData.ConfigFolder,
                         StaticData.InitPluginListName + StaticData.PluginListExtension
                        );

        public static string InternalStartupInstructionPath =>
            Path.Combine(
                         InternalSystemConfigPath,
                         StaticData.ConfigFolder,
                         StaticData.InitStartupInstrsName + StaticData.PluginListExtension
                        );

        /// <summary>
        ///     Returns the Full Path to the Plugin Load Order List File
        /// </summary>
        public static string LoadOrderListFile =>
            Path.Combine(
                         InternalSystemConfigPath,
                         StaticData.ConfigFolder,
                         StaticData.LoadOrderListName + StaticData.PluginListExtension
                        );


        /// <summary>
        ///     Returns the Full Path to the System Temp Directory
        /// </summary>
        public static string SystemTempDirectory => Path.Combine(InternalSystemConfigPath, StaticData.TempFolder);

        /// <summary>
        ///     Returns the Full Path to the Assembly File of the Plugin
        /// </summary>
        /// <param name="name">Plugin Name</param>
        /// <param name="file">Plugin File</param>
        /// <returns>Plugin Assembly File</returns>
        public static string GetPluginAssemblyFile(string name, string file)
        {
            return Path.Combine(GetPluginAssemblyDirectory(name), file);
        }

        /// <summary>
        ///     Returns the Full Path to the Assembly File of the Plugin
        /// </summary>
        /// <param name="data">The plugin Data</param>
        /// <returns>Plugin Assembly File</returns>
        public static string GetPluginAssemblyFile(BasePluginPointer data)
        {
            return GetPluginAssemblyFile(data.PluginName, data.PluginFile);
        }

        /// <summary>
        ///     Returns the Full Path to the System Temp Directory
        /// </summary>
        /// <param name="process">The System Process Name</param>
        /// <returns>System Temp Directory</returns>
        public static string GetSystemProcessTempDirectory(string process)
        {
            return Path.Combine(SystemTempDirectory, process);
        }


        /// <summary>
        ///     Creates all required directories for the Specified Plugin
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        public static void EnsureDirectoriesExist(this BasePluginPointer data)
        {
            Directory.CreateDirectory(GetPluginDirectory(data));
            Directory.CreateDirectory(GetPluginConfigDirectory(data));
            Directory.CreateDirectory(GetPluginAssemblyDirectory(data));
            Directory.CreateDirectory(InternalConfigurationDirectory);
            Directory.CreateDirectory(GetPluginTempDirectory(data));
        }


        internal static void EnsureInternalDirectoriesExist()
        {
            Directory.CreateDirectory(InternalConfigurationDirectory);
            Directory.CreateDirectory(InternalSystemConfigPath);
            Directory.CreateDirectory(SystemTempDirectory);
        }

        internal static void CreateInternalFilesIfMissing()
        {
            if (!File.Exists(InitPluginListFile))
            {
                File.WriteAllText(InitPluginListFile, "");
            }

            if (!File.Exists(LoadOrderListFile))
            {
                File.WriteAllText(LoadOrderListFile, "");
            }

            if (!File.Exists(PluginListFile))
            {
                File.WriteAllText(PluginListFile, "");
            }

            if (!File.Exists(GlobalPluginListFile))
            {
                File.WriteAllText(GlobalPluginListFile, "");
            }
        }

        /// <summary>
        ///     Deletes all directories of the Specified Plugin
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        /// <param name="keepArchive">If true will not Delete the backup archive</param>
        public static void RemovePluginPackageFromDirectoryStructure(BasePluginPointer data, bool keepArchive)
        {
            Directory.Delete(GetPluginDirectory(data), true);
            Directory.Delete(GetPluginTempDirectory(data), true);

            //if (keepArchive && HasArchiveBackup(data))
            //{
            //    File.Delete(GetPluginArchiveBackup(data));
            //}
        }

        /// <summary>
        ///     Reads text from a file, if it fails returns the fallbackContent
        /// </summary>
        /// <param name="path">The Path of the File</param>
        /// <param name="fallbackContent">The Fallback content used if the Loading Failed.</param>
        /// <returns>File Content, On Fail: Fallback Content</returns>
        private static string SaveReadFromFile(string path, string fallbackContent)
        {
            if (!File.Exists(path))
            {
                return fallbackContent;
            }

            return File.ReadAllText(path);
        }

        #region Directories

        /// <summary>
        ///     Returns the Full Path to the Application Entry Directory
        /// </summary>
        /// <returns>Application Entry Directory</returns>
        public static string EntryDirectory =>
            Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath);

        /// <summary>
        ///     Returns the Full Path to the Default System Config Directory
        /// </summary>
        /// <param name="baseDir">Base Directory</param>
        /// <param name="name">Name of the Application</param>
        /// <returns>Default System Config Directory</returns>
        public static string GetDefaultSystemConfigDirectory(string baseDir, string name)
        {
            return Path.Combine(baseDir, name + "-Internal");
        }

        /// <summary>
        ///     Returns the Full Path to the Default System Config Directory
        /// </summary>
        /// <param name="name">Name of the Application</param>
        /// <returns>Default System Config Directory</returns>
        public static string GetDefaultSystemConfigDirectory(string name)
        {
            return GetDefaultSystemConfigDirectory(EntryDirectory, name);
        }

        /// <summary>
        ///     Returns the Full Path to the Default Plugin Directory
        /// </summary>
        /// <param name="baseDir">Base Directory</param>
        /// <param name="name">Name of the Application</param>
        /// <returns>Default Plugin Directory</returns>
        public static string GetDefaultPluginDirectory(string baseDir, string name)
        {
            return Path.Combine(baseDir, name + "-Plugins");
        }

        /// <summary>
        ///     Returns the Full Path to the Default Plugin Directory
        /// </summary>
        /// <param name="name">Name of the Application</param>
        /// <returns>Default Plugin Directory</returns>
        public static string GetDefaultPluginDirectory(string name)
        {
            return GetDefaultPluginDirectory(EntryDirectory, name);
        }


        /// <summary>
        ///     Returns the Full Path to the Plugin Directory
        /// </summary>
        /// <param name="name">Name of the Plugin</param>
        /// <returns>Plugin Directory</returns>
        public static string GetPluginDirectory(string name)
        {
            return Path.Combine(PluginDirectory, name);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Directory
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        /// <returns>Plugin Directory</returns>
        public static string GetPluginDirectory(BasePluginPointer data)
        {
            return GetPluginDirectory(data.PluginName);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Config Directory
        /// </summary>
        /// <param name="name">Name of the Plugin</param>
        /// <returns>Plugin Config Directory</returns>
        public static string GetPluginConfigDirectory(string name)
        {
            return Path.Combine(GetPluginDirectory(name), StaticData.PluginConfigFolder);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Directory
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        /// <returns>Plugin Config Directory</returns>
        public static string GetPluginConfigDirectory(BasePluginPointer data)
        {
            return GetPluginConfigDirectory(data.PluginName);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Assembly Directory
        /// </summary>
        /// <param name="name">Name of the Plugin</param>
        /// <returns>Plugin Assembly Directory</returns>
        public static string GetPluginAssemblyDirectory(string name)
        {
            return Path.Combine(GetPluginDirectory(name), StaticData.PluginBinFolder);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Assembly Directory
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        /// <returns>Plugin Assembly Directory</returns>
        public static string GetPluginAssemblyDirectory(BasePluginPointer data)
        {
            return GetPluginAssemblyDirectory(data.PluginName);
        }

        /// <summary>
        ///     Returns the Full Path to the Internal Configuration Directory
        /// </summary>
        public static string InternalConfigurationDirectory =>
            Path.Combine(InternalSystemConfigPath, StaticData.ConfigFolder);

        /// <summary>
        ///     Returns the Full Path to the Plugin Temp Directory
        /// </summary>
        /// <param name="name">Name of the Plugin</param>
        /// <returns>Plugin Temp Directory</returns>
        public static string GetPluginTempDirectory(string name)
        {
            return Path.Combine(SystemTempDirectory, name);
        }

        /// <summary>
        ///     Returns the Full Path to the Plugin Temp Directory
        /// </summary>
        /// <param name="data">The Pointer Data</param>
        /// <returns>Plugin Temp Directory</returns>
        public static string GetPluginTempDirectory(BasePluginPointer data)
        {
            return GetPluginTempDirectory(data.PluginName);
        }

        #endregion

    }
}