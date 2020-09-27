using System;
using System.IO;
using System.Text;

using PluginSystem.FileSystem;

namespace PluginSystem.Core.Pointer
{
    /// <summary>
    /// Base Plugin Pointer that contains the information of a Plugin
    /// </summary>
    public class BasePluginPointer
    {

        /// <summary>
        /// Simple Constructor
        /// </summary>
        /// <param name="name">Plugin Name</param>
        /// <param name="file">Plugin File</param>
        public BasePluginPointer(string name, string file, string origin, string pluginVersion, string dependencies)
        {
            PluginFile = file;
            PluginName = name;
            PluginOrigin = origin;
            this.dependencies =dependencies;
            PluginVersion = Version.Parse(pluginVersion);
        }

        /// <summary>
        /// Creates a BasePluginPointer with a Plugin Key Pair as input
        /// </summary>
        /// <param name="pluginKeyPair">The key pair as its read from the list</param>
        public BasePluginPointer(string pluginKeyPair)
        {
            string[] parts = pluginKeyPair.Split(
                                                 new[] { StaticData.KeyPairSeparator },
                                                 StringSplitOptions.RemoveEmptyEntries
                                                );
            PluginName = parts[0];
            PluginFile = parts[1];
            PluginOrigin = parts[2];
            PluginVersion = Version.Parse(parts[3]);
            if (parts.Length == 5)
            {
                dependencies = parts[4];
            }
        }

        /// <summary>
        /// The plugin Name
        /// </summary>
        public string PluginName { get; }

        /// <summary>
        /// The Plugin File Path(after the /bin folder in the Plugin Directory.
        /// </summary>
        public string PluginFile { get; }

        public string PluginOrigin { get; }

        public Version PluginVersion { get; }

        private readonly string dependencies;

        public string[] Dependencies => dependencies?.Split(';') ?? new string[0];

        public Uri PluginOriginUri => string.IsNullOrEmpty(PluginOrigin) ? null : new Uri(PluginOrigin);

        /// <summary>
        /// Converts a BasePluginPointer into the Plugin Key Pair Representation
        /// </summary>
        /// <returns>Key Value Pair from the PluginName and PluginFile ({PluginName}|PluginFile}</returns>
        public string ToKeyPair()
        {
            return $"{PluginName}{StaticData.KeyPairSeparator}{PluginFile}{StaticData.KeyPairSeparator}{PluginOrigin}{StaticData.KeyPairSeparator}{PluginVersion}{StaticData.KeyPairSeparator}{dependencies}";
        }

        /// <summary>
        /// To String Implementation Listing all Retrievable Information about the Plugin.
        /// </summary>
        /// <returns>Information Text about this Object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("General:");
            builder.AppendLine("\tIs Initialized: " + PluginManager.IsInitialized);
            builder.AppendLine("\tSystem Config Path: " + PluginPaths.InternalSystemConfigPath);
            builder.AppendLine("\tPlugin Dir: " + PluginPaths.PluginDirectory);
            builder.AppendLine("\tInstalled Packages Path: " + PluginPaths.PluginListFile);
            builder.AppendLine("\tGlobal Packages Path: " + PluginPaths.GlobalPluginListFile);

            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine("Plugin:");
            builder.AppendLine("Name: " + PluginName);
            builder.AppendLine("\tFile: " + PluginName);
            builder.AppendLine("\tPlugin Directory: " + PluginPaths.GetPluginDirectory(PluginName));
            builder.AppendLine("\tPlugin Config Directory: " + PluginPaths.GetPluginConfigDirectory(PluginName));
            builder.AppendLine("\tPlugin Assembly Directory: " + PluginPaths.GetPluginAssemblyDirectory(PluginName));
            builder.AppendLine("\tPlugin Temp Directory: " + PluginPaths.GetPluginTempDirectory(PluginName));

            //builder.AppendLine($"\tPlugin Archive Backup: " + PluginPaths.GetPluginArchiveBackup(PluginName));
            //builder.AppendLine($"\t\tPlugin Archive Backup Exists: " + File.Exists(PluginPaths.GetPluginArchiveBackup(PluginName)));
            builder.AppendLine("\tPlugin Assembly File: " + PluginPaths.GetPluginAssemblyFile(PluginName, PluginFile));
            builder.AppendLine(
                               "\t\tPlugin Assembly File Exists: " +
                               File.Exists(PluginPaths.GetPluginAssemblyFile(PluginName, PluginFile))
                              );

            //builder.AppendLine($"\tPlugin Version File: " + PluginPaths.GetPluginVersionFile(PluginName));
            //builder.AppendLine($"\t\tPlugin Version File Exists: " + File.Exists(PluginPaths.GetPluginVersionFile(PluginName)));
            //builder.AppendLine($"\t\tPlugin Version File Key: " + PluginPaths.GetPluginVersion(PluginName));
            //builder.AppendLine($"\t\tPlugin Version File Value: " + PluginPaths.GetPluginOriginURL(PluginName));
            return builder.ToString();
        }

    }
}