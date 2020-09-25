using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PluginSystem.DefaultPlugins.Formats
{
    internal static class HelperClass
    {

        internal static string[] CopyTo(string inputFolder, string outputFolder)
        {
            Directory.GetDirectories(inputFolder, "*", SearchOption.AllDirectories).ToList()
                     .ForEach(x => Directory.CreateDirectory(x.Replace(inputFolder, outputFolder)));

            List<string> files = Directory.GetFiles(inputFolder, "*", SearchOption.AllDirectories).ToList();

            files.ForEach(x => File.Copy(x, x.Replace(inputFolder, outputFolder), true));
            return files.ToArray();
        }

        internal static string GetPluginVersion(string file)
        {
            FileVersionInfo vi = FileVersionInfo.GetVersionInfo(file);
            return vi.ProductVersion;
        }

    }
}