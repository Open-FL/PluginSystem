using System.Collections.Generic;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;
using PluginSystem.Utility;

namespace PluginSystem.Loading.Ordering
{
    /// <summary>
    /// Helper Class that manages the Load Order
    /// </summary>
    public static class LoadOrder
    {

        /// <summary>
        /// The Load Order Queues
        /// </summary>
        private static string[] Queues => new[] { PluginPaths.LoadOrderListFile };

        /// <summary>
        /// Returns the Load Order List of the Specified Queue
        /// </summary>
        /// <param name="queue">The Queue Specified</param>
        /// <returns>Load Order List</returns>
        public static List<string> GetLoadOrderList(LoadOrderQueue queue)
        {
            return ListHelper.LoadList(Queues[(int) queue]).ToList();
        }

        /// <summary>
        /// Saves the Load Order List of the Specified Queue
        /// </summary>
        /// <param name="queue">The Queue Specified</param>
        /// <param name="value">The new Load Order List</param>
        public static void SetLoadOrderList(LoadOrderQueue queue, List<string> value)
        {
            ListHelper.SaveList(Queues[(int) queue], value.ToArray());
        }

        /// <summary>
        /// Initialize Function
        /// </summary>
        internal static void Initialize()
        {
            PluginManager.AfterInstallPackage += PluginManager_AfterInstallPackage;
            PluginManager.AfterRegisterHost += PluginManager_AfterRegisterHost;
            PluginManager.AfterActivatePackage += PluginManager_AfterActivatePackage;
        }

        private static void PluginManager_AfterActivatePackage(Events.Args.ActivatePackageEventArgs eventArgs)
        {
            List<string> lst = GetLoadOrderList(LoadOrderQueue.Default);
            List<BasePluginPointer> dependentPlugins =
                ListHelper.LoadList(PluginPaths.GlobalPluginListFile).Select(x=>new BasePluginPointer(x)).Where(x => x.Dependencies.Contains(eventArgs.PackagePointer.PluginName)).ToList();

            foreach (BasePluginPointer dependentPlugin in dependentPlugins)
            {
                lst.Remove(dependentPlugin.PluginName);
                lst.Add(dependentPlugin.PluginName);
            }
            SetLoadOrderList(LoadOrderQueue.Default, lst);
        }

        /// <summary>
        /// Makes sure that the Load Order is set for each plugin that is installed.
        /// </summary>
        private static void PluginManager_AfterRegisterHost(Events.Args.RegisterHostEventArgs eventArgs)
        {
            List<string> lst = GetLoadOrderList(LoadOrderQueue.Default);
            List<string> l = ListHelper.LoadList(PluginPaths.GlobalPluginListFile)
                                       .Select(x => x.Split(StaticData.KeyPairSeparator).First()).ToList();
            lst.AddRange(l.Where(x => !lst.Contains(x)));
            SetLoadOrderList(LoadOrderQueue.Default, lst);
        }

        /// <summary>
        /// Add to load order list after installing the Package
        /// </summary>
        private static void PluginManager_AfterInstallPackage(Events.Args.InstallPackageEventArgs eventArgs)
        {
            List<string> lo = GetLoadOrderList(LoadOrderQueue.Default);
            if (!lo.Contains(eventArgs.Data.PluginName))
            {
                lo.Add(eventArgs.Data.PluginName);
                SetLoadOrderList(LoadOrderQueue.Default, lo);
            }
        }

        /// <summary>
        /// Sorts the Input lists so that the Highest Priority Plugin is at the top
        /// </summary>
        /// <param name="ptr">Pointer List</param>
        public static void SortList(List<PluginAssemblyPointer> ptr)
        {
            List<string> loadOrder = GetLoadOrderList(LoadOrderQueue.Default);
            ptr.Sort(
                     (pointer, pluginPointer) =>
                     {
                         int idxA = loadOrder.IndexOf(pointer.PluginName);
                         if (idxA == -1)
                         {
                             idxA = int.MaxValue;
                         }

                         int idxB = loadOrder.IndexOf(pointer.PluginName);
                         if (idxB == -1)
                         {
                             idxB = int.MaxValue;
                         }

                         return idxA.CompareTo(idxB);
                     }
                    );
        }

        /// <summary>
        /// Checks if the Plugin is set in the specified queue.
        /// </summary>
        /// <param name="queue">The Queue</param>
        /// <param name="pluginName">The Plugin to be checked</param>
        /// <returns>True of the Plugin is Contained in the Queue</returns>
        public static bool Contains(LoadOrderQueue queue, string pluginName)
        {
            return GetLoadOrderList(queue).Contains(pluginName);
        }

        /// <summary>
        /// Moves the Specified Plugin up one Entry
        /// </summary>
        /// <param name="queue">The Queue</param>
        /// <param name="pluginName">The Plugin to be moved.</param>
        public static void MoveUp(LoadOrderQueue queue, string pluginName)
        {
            List<string> lst = GetLoadOrderList(queue);
            bool contains = lst.Contains(pluginName);
            if (!contains)
            {
                return;
            }

            int idx = lst.IndexOf(pluginName);

            if (idx == 0)
            {
                return;
            }

            int newIdx = idx - 1;
            lst.RemoveAt(idx);
            lst.Insert(newIdx, pluginName);
            SetLoadOrderList(queue, lst);
        }

        /// <summary>
        /// Moves the Specified Plugin down one Entry
        /// </summary>
        /// <param name="queue">The Queue</param>
        /// <param name="pluginName">The Plugin to be moved.</param>
        public static void MoveDown(LoadOrderQueue queue, string pluginName)
        {
            List<string> lst = GetLoadOrderList(queue);
            bool contains = lst.Contains(pluginName);
            if (!contains)
            {
                return;
            }

            int idx = lst.IndexOf(pluginName);
            if (idx == lst.Count - 1)
            {
                return;
            }

            int newIdx = idx + 1;
            lst.RemoveAt(idx);
            lst.Insert(newIdx, pluginName);
            SetLoadOrderList(queue, lst);
        }

        /// <summary>
        /// Moves the Specified Plugin to the top
        /// </summary>
        /// <param name="queue">The Queue</param>
        /// <param name="pluginName">The Plugin to be moved.</param>
        /// <param name="addIfNotFound">When not found add the plugin to the Load Order List</param>
        public static void MoveToTop(LoadOrderQueue queue, string pluginName, bool addIfNotFound = false)
        {
            List<string> lst = GetLoadOrderList(queue);
            bool contains = lst.Contains(pluginName);
            if (!addIfNotFound && !contains)
            {
                return;
            }

            if (contains)
            {
                lst.Remove(pluginName);
            }

            lst.Insert(0, pluginName);
            SetLoadOrderList(queue, lst);
        }

        /// <summary>
        /// Moves the Specified Plugin to the bottom
        /// </summary>
        /// <param name="queue">The Queue</param>
        /// <param name="pluginName">The Plugin to be moved.</param>
        /// <param name="addIfNotFound">When not found add the plugin to the Load Order List</param>
        public static void MoveToBottom(LoadOrderQueue queue, string pluginName, bool addIfNotFound = false)
        {
            List<string> lst = GetLoadOrderList(queue);
            bool contains = lst.Contains(pluginName);
            if (!addIfNotFound && !contains)
            {
                return;
            }

            if (contains)
            {
                lst.Remove(pluginName);
            }

            lst.Add(pluginName);
            SetLoadOrderList(queue, lst);
        }

    }
}