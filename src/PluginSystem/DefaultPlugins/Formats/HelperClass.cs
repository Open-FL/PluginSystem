using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.DefaultPlugins.Formats.PackageData;
using PluginSystem.DefaultPlugins.Formats.Packer;

namespace PluginSystem.DefaultPlugins.Formats
{
    public static class HelperClass
    {

        internal static void ReloadDefaultPlugins()
        {
            PluginManager.AddPlugin(
                                    FolderPackerFormat.Embedded(),
                                    new PluginAssemblyPointer(
                                                              "plugin-format-dir-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
            PluginManager.AddPlugin(
                                    new ZipPackerFormat(),
                                    new PluginAssemblyPointer(
                                                              "plugin-format-zip-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
            PluginManager.AddPlugin(
                                    VSBuildPlugin.Embedded(),
                                    new PluginAssemblyPointer(
                                                              "plugin-format-vs-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
            PluginManager.AddPlugin(
                                    PlainTextDataFormat.Embedded(),
                                    new PluginAssemblyPointer(
                                                              "plugin-ptr-format-plain-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
            PluginManager.AddPlugin(
                                    new DLLPackerFormat(),
                                    new PluginAssemblyPointer(
                                                              "plugin-format-dll-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
            PluginManager.AddPlugin(
                                    new URLPackerFormat(),
                                    new PluginAssemblyPointer(
                                                              "plugin-format-url-packer",
                                                              "",
                                                              "",
                                                              Assembly.GetExecutingAssembly().GetName().Version
                                                                      .ToString(),
                                                              PluginManager.PluginHost
                                                             )
                                   );
        }


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