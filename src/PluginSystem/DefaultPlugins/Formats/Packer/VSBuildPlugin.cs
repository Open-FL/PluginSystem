using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;
using PluginSystem.FileSystem.Packer;
using PluginSystem.Utility;

namespace PluginSystem.DefaultPlugins.Formats.Packer
{
    public class VSBuildPlugin : APluginPackerFormat
    {

        private readonly bool isEmbedded;

        public VSBuildPlugin()
        {
        }

        private VSBuildPlugin(bool isEmbedded)
        {
            this.isEmbedded = isEmbedded;
        }

        public static VSBuildPlugin Embedded()
        {
            return new VSBuildPlugin(true);
        }


        public override bool CanLoad(string file)
        {
            return file.EndsWith(".csproj");
        }

        public override string[] Pack(string inputFolder, string outputFolder)
        {
            throw new NotImplementedException();
        }

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            if (isEmbedded)
            {
                return;
            }

            List<string> initList = ListHelper.LoadList(PluginPaths.InitPluginListFile).ToList();
            if (!initList.Contains(PluginPaths.GetPluginAssemblyFile(ptr)))
            {
                initList.Add(PluginPaths.GetPluginAssemblyFile(ptr));
                ListHelper.SaveList(PluginPaths.InitPluginListFile, initList.ToArray());
            }
        }

        public void BeforeInit()
        {
            PluginPacker.AddPackerFormat(this);
        }

        public override void Unpack(string file, string outputDir)
        {
            string binDir = Path.Combine(outputDir, StaticData.PluginBinFolder);
            Directory.CreateDirectory(binDir);
            string targetDir = Path.Combine(Path.GetDirectoryName(file), "bin", "Debug");
            string pluginName = Path.GetFileNameWithoutExtension(file);
            string pluginAssembly = pluginName + ".dll";
            string[] dirs = Directory.GetDirectories(targetDir, "*", SearchOption.TopDirectoryOnly);
            string corePath = dirs.FirstOrDefault(x => Path.GetFileName(x).StartsWith("netstandard"));
            if (corePath != null)
            {
                targetDir = Path.Combine(targetDir, corePath);
            }

            File.WriteAllText(
                              Path.Combine(outputDir, "info.txt"),
                              $"{pluginName}|{pluginAssembly}|{Path.Combine(binDir, pluginAssembly)}|9.9.9.9"
                             );
            File.Copy(Path.Combine(targetDir, pluginAssembly), Path.Combine(binDir, pluginAssembly));
        }

    }
}