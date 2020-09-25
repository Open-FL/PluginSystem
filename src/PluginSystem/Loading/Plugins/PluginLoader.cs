using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using PluginSystem.Core;
using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;
using PluginSystem.Exceptions;
using PluginSystem.FileSystem;

namespace PluginSystem.Loading.Plugins
{
    public static class PluginLoader
    {

        /// <summary>
        /// Adds All Plugins that are compatible to the Host Specified in data.Host
        /// </summary>
        /// <param name="data">The Assembly Data</param>
        public static void AddPluginsFromLoaderResult(PluginAssemblyPointer data)
        {
            PluginLoaderResult res = GetAllPluginsCompatibleForHost(data);

            foreach (IPlugin re in res.LoadedPlugins)
            {
                PluginManager.AddPlugin(re, data);
            }
        }

        /// <summary>
        /// Returns Instances of the specified types.
        /// </summary>
        /// <typeparam name="T">The Common Type between the types in the types array.</typeparam>
        /// <param name="types">The types that should be created.</param>
        /// <returns>List of Instantiated Type Instances</returns>
        internal static List<T> GetAllTypeInstances<T>(Type[] types)
        {
            return FindTypesWithInterface(types, typeof(T)).Select(x => (T) Activator.CreateInstance(x)).ToList();
        }

        /// <summary>
        /// Loads all Plugins that are compatible to a Host
        /// </summary>
        /// <param name="data">The Pointer to the Package that should be searched.</param>
        /// <returns>Plugin Loader Result containing the Plugins</returns>
        public static PluginLoaderResult GetAllPluginsCompatibleForHost(PluginAssemblyPointer data)
        {
            Assembly asm = SaveLoadFrom(data);
            if (asm == null)
            {
                return PluginLoaderResult.EmptyOrError;
            }

            List<IPlugin> plugins = CreateTypesFromInterface(asm, data.Host);

            return new PluginLoaderResult { LoadedPlugins = plugins.ToArray() };
        }


        public static List<IPlugin> CreateTypesFromInterface(Assembly asm, IPluginHost host)
        {
            Type[] pluginTypes = FindTypesWithInterface(asm, typeof(IPlugin));
            List<IPlugin> plugins = new List<IPlugin>();
            foreach (Type plugin in pluginTypes)
            {
                try
                {
                    IPlugin p = (IPlugin) Activator.CreateInstance(plugin);
                    if (p.SatisfiesHostType(host))
                    {
                        plugins.Add(p);
                    }
                }
                catch (Exception e)
                {
                    PluginManager.SendError(
                                            new TypeInstantiationReflectionException(
                                                                                     "Could not Instantiate the Plugin",
                                                                                     plugin,
                                                                                     e
                                                                                    )
                                           );
                }
            }

            return plugins;
        }

        /// <summary>
        /// Loads a C# Assembly From file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Loaded Assembly</returns>
        internal static Assembly SaveLoadFrom(string file)
        {
            try
            {
                Assembly asm = Assembly.LoadFrom(file);
                return asm;
            }
            catch (Exception e)
            {
                PluginManager.SendError(
                                        new AssemblyLoadException(
                                                                  "The Assembly could not be located or loaded",
                                                                  file,
                                                                  e
                                                                 )
                                       );
                return null;
            }
        }

        /// <summary>
        /// Loads a C# Assembly from a Pointer
        /// </summary>
        /// <param name="data">Pointer Data</param>
        /// <returns>Loaded Assembly</returns>
        internal static Assembly SaveLoadFrom(PluginAssemblyPointer data)
        {
            return SaveLoadFrom(PluginPaths.GetPluginAssemblyFile(data));
        }


        /// <summary>
        /// Returns all types that inherit a specific interface, are not abstract and not an interface themselves.
        /// </summary>
        /// <param name="types">Types to check</param>
        /// <param name="interfaceT">Target Type</param>
        /// <returns>List of types that can be assigned to interfaceT</returns>
        internal static Type[] FindTypesWithInterface(Type[] types, Type interfaceT)
        {
            if (!interfaceT.IsInterface)
            {
                return new Type[0];
            }

            try
            {
                return types.Where(
                                   pluginType =>
                                       !pluginType.IsInterface &&
                                       !pluginType.IsAbstract &&
                                       interfaceT.IsAssignableFrom(
                                                                   pluginType
                                                                  ) /*&& pluginType.GetInterfaces().Any(interfaceType => interfaceType == interfaceT)*/
                                  ).ToArray();
            }
            catch (Exception e)
            {
                PluginManager.SendError(
                                        new TypeFinderReflectionException(
                                                                          $"Could not Complete Search for Classes of Type {interfaceT.Name}",
                                                                          e
                                                                         )
                                       );
                return new Type[0];
            }
        }

        internal static Type[] FindTypesWithAttribute<T>(Type[] types) where T : Attribute
        {
            try
            {
                foreach (Type type in types)
                {
                    T[] attribs = type.GetCustomAttributes<T>(true).ToArray();
                }

                return types.Where(x => x.GetCustomAttributes<T>(true).Count() != 0).ToArray();
            }
            catch (Exception e)
            {
                PluginManager.SendError(
                                        new TypeFinderReflectionException(
                                                                          $"Could not Complete Search for Classes with Attribute: {typeof(T)}",
                                                                          e
                                                                         )
                                       );
                return new Type[0];
            }
        }

        /// <summary>
        /// Returns all types that inherit a specific interface, are not abstract and not an interface themselves.
        /// </summary>
        /// <param name="asm">Assembly containing the Types to check</param>
        /// <param name="interfaceT">Target Type</param>
        /// <returns>List of types that can be assigned to interfaceT</returns>
        internal static Type[] FindTypesWithInterface(Assembly asm, Type interfaceT)
        {
            //PluginManager.SendLog($"Finding Classes of type {interfaceT.Name} in Assemblies..");
            Type[] ret = FindTypesWithInterface(asm.GetTypes(), interfaceT);

            //PluginManager.SendLog($"Found {ret.Length} Classes.");
            return ret;
        }

    }
}