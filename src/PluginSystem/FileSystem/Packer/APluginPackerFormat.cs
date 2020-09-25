using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.Utility;

namespace PluginSystem.FileSystem.Packer
{
    /// <summary>
    /// Abstract Class that can be implemented to create Custom Plugin Packer Formats
    /// </summary>
    public abstract class APluginPackerFormat : APlugin<PluginSystemHost>
    {

        /// <summary>
        /// Gets Called when this Plugin is attached to a Host.
        /// </summary>
        /// <param name="ptr">The Pointer that is used to Query File Paths and other Plugin Specific Data</param>
        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            PluginPacker.AddPackerFormat(this);
        }

        /// <summary>
        /// Gets Called when this Plugin is detached from a host
        /// </summary>
        public override void OnUnload()
        {
            base.OnUnload();
            PluginPacker.RemovePackerFormat(this);
        }

        /// <summary>
        /// Checks if the Format can Load the Specified File
        /// </summary>
        /// <param name="file">File to check.</param>
        /// <returns>True if this Format can load the Type of File</returns>
        public abstract bool CanLoad(string file);

        /// <summary>
        /// Packs the Specified input folder and outputs the files to the output folder.
        /// </summary>
        /// <param name="inputFolder">Input Folder</param>
        /// <param name="outputFolder">Output Folder</param>
        /// <returns>The Files that were Built</returns>
        public abstract string[] Pack(string inputFolder, string outputFolder);

        /// <summary>
        /// Unpacks a File into an Output Directory
        /// </summary>
        /// <param name="file">File to Unpack</param>
        /// <param name="outputDirectory">Output Directory</param>
        public abstract void Unpack(string file, string outputDirectory);

    }
}