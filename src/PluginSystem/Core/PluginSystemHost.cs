using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;

namespace PluginSystem.Core
{
    /// <summary>
    ///     Host Class used by the Plugin System.
    ///     Is used to add Formats and other Extensions as Plugins
    /// </summary>
    public sealed class PluginSystemHost : IPluginHost
    {

        /// <summary>
        ///     Is used by the Plugin System to determine if a Plugin is Compatible to this Host
        /// </summary>
        /// <param name="plugin">The Plugin to check against</param>
        /// <returns>True if the Host is Supporting this Plugin</returns>
        public bool IsAllowedPlugin(IPlugin plugin)
        {
            return true;
        }


        /// <summary>
        ///     Is called when a Plugin gets loaded into this host.
        /// </summary>
        /// <param name="plugin">The plugin that gets loaded.</param>
        /// <param name="ptr">The Plugin Pointer Data</param>
        public void OnPluginLoad(IPlugin plugin, BasePluginPointer ptr)
        {
        }

        /// <summary>
        ///     Is called when a Plugin gets unloaded from this host.
        /// </summary>
        /// <param name="plugin">The plugin that gets loaded.</param>
        public void OnPluginUnload(IPlugin plugin)
        {
        }

    }
}