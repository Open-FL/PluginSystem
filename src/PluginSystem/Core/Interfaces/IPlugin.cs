using PluginSystem.Core.Pointer;

namespace PluginSystem.Core.Interfaces
{
    /// <summary>
    /// Interface that needs to be inherited by every Plugin
    /// </summary>
    public interface IPlugin
    {
        bool IsMainPlugin { get; }
        bool HasIO { get; }
        string Name { get; }
        /// <summary>
        /// Gets Called when this Plugin is attached to a Host.
        /// </summary>
        /// <param name="ptr">The Pointer that is used to Query File Paths and other Plugin Specific Data</param>
        void OnLoad(PluginAssemblyPointer ptr);

        /// <summary>
        /// Gets Called when this Plugin is detached from a host
        /// </summary>
        void OnUnload();

        /// <summary>
        /// Is used by the Plugin System to determine if a Plugin is Compatible to a Host
        /// </summary>
        /// <param name="potentialHost">The Host to check against</param>
        /// <returns>True if the Plugin is Supporting this Host</returns>
        bool SatisfiesHostType(IPluginHost potentialHost);

    }
}