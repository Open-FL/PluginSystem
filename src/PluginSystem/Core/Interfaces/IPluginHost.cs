using PluginSystem.Core.Pointer;

namespace PluginSystem.Core.Interfaces
{
    /// <summary>
    ///     Interface that needs to be inherited by every PluginHost
    /// </summary>
    public interface IPluginHost
    {

        /// <summary>
        ///     Is used by the Plugin System to determine if a Plugin is Compatible to this Host
        /// </summary>
        /// <param name="plugin">The Plugin to check against</param>
        /// <returns>True if the Host is Supporting this Plugin</returns>
        bool IsAllowedPlugin(IPlugin plugin);

        /// <summary>
        ///     Is called when a Plugin gets loaded into this host.
        /// </summary>
        /// <param name="plugin">The plugin that gets loaded.</param>
        /// <param name="ptr">The Plugin Pointer Data</param>
        void OnPluginLoad(IPlugin plugin, BasePluginPointer ptr);

        /// <summary>
        ///     Is called when a Plugin gets unloaded from this host.
        /// </summary>
        /// <param name="plugin">The plugin that gets loaded.</param>
        /// <param name="ptr">The Plugin Pointer Data</param>
        void OnPluginUnload(IPlugin plugin);

    }
}