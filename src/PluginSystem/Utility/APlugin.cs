using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;

namespace PluginSystem.Utility
{
    /// <summary>
    /// Abstract IPlugin Implementation
    /// </summary>
    /// <typeparam name="Host">A type that implements the IPluginHost Interface</typeparam>
    public abstract class APlugin<Host> : IPlugin
        where Host : IPluginHost
    {

        public virtual string Name => GetType().Name;

        public virtual bool IsMainPlugin => false;

        public virtual bool HasIO => true;
        /// <summary>
        /// The Plugin Pointer that is associated to this Plugin/Host combo
        /// </summary>
        public PluginAssemblyPointer PluginAssemblyData { get; private set; }

        /// <summary>
        /// The Host of Type T
        /// </summary>
        public Host PluginHost => (Host) PluginAssemblyData.Host;

        /// <summary>
        /// Is used by the Plugin System to determine if a Plugin is Compatible to a Host
        /// </summary>
        /// <param name="potentialHost">The Host to check against</param>
        /// <returns>True if the Plugin is Supporting this Host</returns>
        public virtual bool SatisfiesHostType(IPluginHost potentialHost)
        {
            return potentialHost != null &&
                   !GetType().IsInstanceOfType(
                                               potentialHost
                                              ) && //If we happen to be a Plugin host and a Plugin we can not attach to a class that is of the type of our own.
                   potentialHost is Host;
        }


        /// <summary>
        /// Gets Called when this Plugin is attached to a Host.
        /// </summary>
        /// <param name="ptr">The Pointer that is used to Query File Paths and other Plugin Specific Data</param>
        public virtual void OnLoad(PluginAssemblyPointer ptr)
        {
            PluginAssemblyData = ptr;
        }

        /// <summary>
        /// Gets Called when this Plugin is detached from a host
        /// </summary>
        public virtual void OnUnload()
        {
            PluginAssemblyData = null;
        }

    }
}