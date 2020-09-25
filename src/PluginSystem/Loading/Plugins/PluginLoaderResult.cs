namespace PluginSystem.Loading.Plugins
{
    /// <summary>
    /// Contains the Result of a Plugin Load Operation
    /// </summary>
    public struct PluginLoaderResult
    {

        /// <summary>
        /// The Loaded Plugins
        /// </summary>
        public object[] LoadedPlugins;

        /// <summary>
        /// Helper Property that defines the Default Error Return Value
        /// </summary>
        public static PluginLoaderResult EmptyOrError => new PluginLoaderResult { LoadedPlugins = new object[0] };

    }
}