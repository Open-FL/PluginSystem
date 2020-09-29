using System;
using System.Diagnostics;
using System.IO;

using PluginSystem.Core;
using PluginSystem.FileSystem.Packer;

namespace PluginSystem.DefaultPlugins.Formats.Packer
{
    public class DLLPackerFormat : APluginPackerFormat
    {

        public override bool CanLoad(string file)
        {
            return File.Exists(file) && file.EndsWith(".dll");
        }

        public override string[] Pack(string inputFolder, string outputFolder)
        {
            throw new NotImplementedException();
        }

        public override void Unpack(string file, string outputDirectory)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            string binDir = Path.Combine(outputDirectory, StaticData.PluginBinFolder);
            string confDir = Path.Combine(outputDirectory, StaticData.PluginConfigFolder);
            string targetAssembly = Path.Combine(binDir, name + ".dll");
            Directory.CreateDirectory(binDir);
            Directory.CreateDirectory(confDir);
            File.Copy(file, targetAssembly);

            string pluginAssembly = Path.GetFileName(file);


            File.WriteAllText(
                              Path.Combine(outputDirectory, "info.txt"),
                              $"{name}|{pluginAssembly}|{file}|{FileVersionInfo.GetVersionInfo(targetAssembly).FileVersion}"
                             );
        }

    }
}