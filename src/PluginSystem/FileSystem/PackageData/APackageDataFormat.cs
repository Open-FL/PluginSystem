using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.Utility;

namespace PluginSystem.FileSystem.PackageData
{
    /// <summary>
    ///     Abstract Class that can be implemented to create Custom Package Data Formats
    /// </summary>
    public abstract class APackageDataFormat : APlugin<PluginSystemHost>
    {

        /// <summary>
        ///     Gets Called when this Plugin is attached to a Host.
        /// </summary>
        /// <param name="ptr">The Pointer that is used to Query File Paths and other Plugin Specific Data</param>
        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            PackageDataManager.AddPackerFormat(this);
        }

        /// <summary>
        ///     Gets Called when this Plugin is detached from a host
        /// </summary>
        public override void OnUnload()
        {
            base.OnUnload();
            PackageDataManager.RemovePackerFormat(this);
        }

        /// <summary>
        ///     Checks if the Format can Load the Specified File
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>True if this Format can load the Type of File</returns>
        public abstract bool CanLoad(string directory);

        /// <summary>
        ///     Loads the BasePluginPointer from a Directory
        /// </summary>
        /// <param name="folder">The Folder containing the Unpacked Files</param>
        /// <returns>The Files that were Built</returns>
        public abstract BasePluginPointer LoadData(string folder);

        /// <summary>
        ///     Saves the BasePluginPointer to a Directory
        /// </summary>
        /// <param name="data">Data to Save</param>
        /// <param name="outputFolder">Output Folder</param>
        public abstract void SaveData(BasePluginPointer data, string outputFolder);

        /// <summary>
        ///     Installs the Specified folder contents into the Directories of the specified pointer
        /// </summary>
        /// <param name="ptr">The Pointer</param>
        /// <param name="folder">The Folder with contents</param>
        public abstract void Install(BasePluginPointer ptr, string folder);

    }
}