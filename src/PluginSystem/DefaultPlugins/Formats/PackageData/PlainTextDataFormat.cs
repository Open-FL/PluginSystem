using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;
using PluginSystem.FileSystem.PackageData;
using PluginSystem.Utility;

namespace PluginSystem.DefaultPlugins.Formats.PackageData
{
    public class PlainTextDataFormat : APackageDataFormat
    {

        private readonly bool isEmbedded;

        public PlainTextDataFormat()
        {
        }

        private PlainTextDataFormat(bool isEmbedded)
        {
            this.isEmbedded = isEmbedded;
        }

        public static PlainTextDataFormat Embedded()
        {
            return new PlainTextDataFormat(true);
        }

        private string GetDataPath(string dir)
        {
            return Path.Combine(dir, "info.txt");
        }

        private string GetConfigDir(string dir)
        {
            return Path.Combine(dir, StaticData.PluginConfigFolder);
        }

        private string GetBinPath(string dir)
        {
            return Path.Combine(dir, StaticData.PluginBinFolder);
        }

        public override bool CanLoad(string directory)
        {
            return File.Exists(GetDataPath(directory));
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

        public override BasePluginPointer LoadData(string folder)
        {
            string kvp = File.ReadAllText(GetDataPath(folder));
            return new BasePluginPointer(kvp);
        }

        public override void Install(BasePluginPointer ptr, string folder)
        {
            string cdir = GetConfigDir(folder);
            string bdir = GetBinPath(folder);
            if (Directory.Exists(cdir))
            {
                HelperClass.CopyTo(cdir, PluginPaths.GetPluginConfigDirectory(ptr));
            }

            if (Directory.Exists(bdir))
            {
                HelperClass.CopyTo(bdir, PluginPaths.GetPluginAssemblyDirectory(ptr));
            }
        }

        public override void SaveData(BasePluginPointer data, string outputFolder)
        {
            File.WriteAllText(GetDataPath(outputFolder), data.ToKeyPair());
        }

    }
}