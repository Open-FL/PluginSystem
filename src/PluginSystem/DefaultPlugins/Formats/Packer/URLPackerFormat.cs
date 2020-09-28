using System;
using System.IO;

using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem.Packer;
using PluginSystem.Updating;

namespace PluginSystem.DefaultPlugins.Formats.Packer
{
    public class URLPackerFormat : APluginPackerFormat
    {

        public override bool CanLoad(string file)
        {
            if (file == "" || !file.EndsWith("info.txt")) return false;
            return WebPointerUpdateChecker.IsWebPointer(new Uri(file));
        }

        public override string[] Pack(string inputFolder, string outputFolder)
        {
            throw new System.NotImplementedException();
        }

        public override void Unpack(string file, string outputDirectory)
        {
            BasePluginPointer ptr = WebPointerUpdateChecker.GetPointer(file);
            string zip = WebPointerUpdateChecker.DownloadFile(ptr);
            ZipPackerFormat packer = new ZipPackerFormat();
            packer.Unpack(zip, outputDirectory);
            File.Delete(zip);
        }

    }
}