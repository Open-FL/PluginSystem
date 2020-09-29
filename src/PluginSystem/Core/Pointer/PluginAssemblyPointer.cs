using System.Text;

using PluginSystem.Core.Interfaces;

namespace PluginSystem.Core.Pointer
{
    /// <summary>
    ///     Assembly Plugin Pointer that contains the information of a Plugin that is Loaded in a Host
    /// </summary>
    public class PluginAssemblyPointer : BasePluginPointer
    {

        /// <summary>
        ///     Simple Constructor
        /// </summary>
        /// <param name="name">Plugin Name</param>
        /// <param name="file">Plugin File</param>
        /// <param name="host">The Plugin Host</param>
        public PluginAssemblyPointer(string name, string file, string origin, string version, IPluginHost host) : base(
                                                                                                                       name,
                                                                                                                       file,
                                                                                                                       origin,
                                                                                                                       version,
                                                                                                                       ""
                                                                                                                      )
        {
            Host = host;
        }

        /// <summary>
        ///     Creates a BasePluginPointer with a Plugin Key Pair as input
        /// </summary>
        /// <param name="pluginKeyPair">Plugin Key Pair</param>
        /// <param name="host">The Plugin Host</param>
        public PluginAssemblyPointer(string pluginKeyPair, IPluginHost host) : base(pluginKeyPair)
        {
            Host = host;
        }

        /// <summary>
        ///     The Host that the Plugin is loaded in
        /// </summary>
        public IPluginHost Host { get; }

        /// <summary>
        ///     To String Implementation Listing all Retrievable Information about the Plugin.
        /// </summary>
        /// <returns>Information Text about this Object.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Host:");
            builder.AppendLine(Host.ToString());

            builder.AppendLine();
            builder.AppendLine();

            builder.AppendLine(base.ToString());

            return builder.ToString();
        }

    }
}