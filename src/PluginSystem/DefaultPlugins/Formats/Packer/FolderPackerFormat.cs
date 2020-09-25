using System.IO;

using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem.Packer;
using PluginSystem.Loading.Ordering;

namespace PluginSystem.DefaultPlugins.Formats.Packer
{
    public class FolderPackerFormat : APluginPackerFormat
    {

        private readonly bool isEmbedded;

        public FolderPackerFormat()
        {
        }

        private FolderPackerFormat(bool isEmbedded)
        {
            this.isEmbedded = isEmbedded;
        }

        public static FolderPackerFormat Embedded()
        {
            return new FolderPackerFormat(true);
        }


        public override bool CanLoad(string file)
        {
            return Directory.Exists(file);
        }

        public override string[] Pack(string inputFolder, string outputFolder)
        {
            //Simple Copy input folder to output folder
            return HelperClass.CopyTo(inputFolder, outputFolder);
        }

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            //Call the Base OnLoad to Automatically do the registration.
            base.OnLoad(ptr);

            if (!isEmbedded)

                //Make sure that the Folder Packer is listed in the Init List and its one of the first things that get loaded.
            {
                LoadOrder.MoveToTop(LoadOrderQueue.Default, ptr.PluginName, true);
            }
        }

        public override void Unpack(string file, string outputDir)
        {
            //Simple Copy input folder to output folder
            HelperClass.CopyTo(file, outputDir);
        }

    }
}