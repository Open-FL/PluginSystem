using System;
using System.IO;
using System.IO.Compression;

using PluginSystem.FileSystem.Packer;

namespace PluginSystem.DefaultPlugins.Formats.Packer
{
    public class ZipPackerFormat : APluginPackerFormat
    {

        public override bool CanLoad(string file)
        {
            try
            {
                ZipFile.Open(file, ZipArchiveMode.Read).Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override string[] Pack(string inputFolder, string outputFolder)
        {
            string path = Path.Combine(outputFolder, "Plugin.zip");
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            ZipFile.CreateFromDirectory(inputFolder, path);
            return new[] { path };
        }

        public override void Unpack(string file, string outputDir)
        {
            ZipFile.ExtractToDirectory(file, outputDir);
        }

    }
}