using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;
using PluginSystem.Events;
using PluginSystem.Events.Args;
using PluginSystem.Exceptions;
using PluginSystem.FileSystem;
using PluginSystem.FileSystem.PackageData;
using PluginSystem.FileSystem.Packer;
using PluginSystem.Loading.Ordering;
using PluginSystem.Loading.Plugins;
using PluginSystem.Updating;
using PluginSystem.Utility;

namespace PluginSystem.Core
{
    /// <summary>
    /// Main Class used to Interface with the PluginSystem
    /// </summary>
    public static class PluginManager
    {

        /// <summary>
        /// Map of Plugins that are loaded for a Plugin Host
        /// </summary>
        internal static readonly Dictionary<IPluginHost, List<IPlugin>> LoadedPlugins =
            new Dictionary<IPluginHost, List<IPlugin>>();

        internal static readonly Dictionary<IPlugin, PluginAssemblyPointer> PointerMap = new Dictionary<IPlugin, PluginAssemblyPointer>();

        /// <summary>
        /// The Plugin System Host instance that is used to be able to register a Plugin targeting this Library
        /// </summary>
        public static PluginSystemHost PluginHost { get; private set; }

        /// <summary>
        /// Flag that indicates if the Plugin System is initialized.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// Cleans all Temp Files from plugins and the System itself.
        /// </summary>
        public static void CleanTempDirectory()
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }

            SendLog("Cleaning Temp Directories.");
            CleanTempFolderEventArgs args = new CleanTempFolderEventArgs();
            OnCleanTempFolder?.Invoke(args);
            if (args.Cancel)
            {
                return;
            }

            if (Directory.Exists(PluginPaths.SystemTempDirectory))
            {
                Directory.Delete(PluginPaths.SystemTempDirectory, true);
            }

            AfterCleanTempFolder?.Invoke();
        }


        public static void Initialize(
            string rootPath, string internalConfigPath, string pluginDirectory, Func<string, string, bool> updateDialog, string staticDataConfig = null)
        {
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            Initialize(
                       Path.Combine(rootPath, internalConfigPath),
                       Path.Combine(rootPath, pluginDirectory),
                       updateDialog,
                       staticDataConfig
                      );
        }

        /// <summary>
        /// Initializes the Plugin System.
        /// </summary>
        /// <param name="internalConfigPath">The Path that is used by internal config files by the Plugin System</param>
        /// <param name="pluginDirectory">The Path used as "Install Directory" for Plugins/Packages</param>
        public static void Initialize(string internalConfigPath, string pluginDirectory, Func<string, string, bool> updateDialog, string staticDataConfig = null)
        {
            if (IsInitialized)
            {
                throw new Exception("Can not Initialize the Plugin System Twice");
            }

            SendLog("Initializing Plugin System");

            //TODO: Process Things like updates before the plugin system loads the libraries.

            PluginPaths.InternalSystemConfigPath = Path.GetFullPath(internalConfigPath);
            PluginPaths.PluginDirectory = Path.GetFullPath(pluginDirectory);
            PluginPaths.EnsureInternalDirectoriesExist();
            PluginPaths.CreateInternalFilesIfMissing();
            ErrorHandler.Initialize();

            LoadOrder.Initialize();
            if (staticDataConfig != null && File.Exists(staticDataConfig))
            {
                StaticData.SetState(File.ReadAllText(staticDataConfig));
            }


            IsInitialized = true;

            SendLog("Updating..");

            ListHelper.LoadList(PluginPaths.PluginListFile).Select(x => new BasePluginPointer(x)).ToList()
                      .ForEach(x => UpdateManager.CheckAndUpdate(x, updateDialog));

                               SendLog("Registering System Host..");
            PluginHost = new PluginSystemHost();
            LoadPlugins(PluginHost);
            SendLog("Registered System Host..");
            SendLogDivider();

            //Everything Finished
            OnInitialized?.Invoke();
            SendLog("Initialization Complete.");
            SendLogDivider();
        }

        #region Events

        /// <summary>
        /// Default Log Handler, (uses Console.WriteLine)
        /// </summary>
        /// <param name="eventArgs">Log Message Event Argument</param>
        private static void DefaultLogHandler(LogMessageEventArgs eventArgs)
        {
            Console.WriteLine("Log: " + eventArgs.Message);
        }

        /// <summary>
        /// OnError Event Delegate
        /// </summary>
        private static event PluginEvents.OnErrorEvent OnError;

        /// <summary>
        /// Sends an Error Event through the PluginSystem OnError Handler
        /// </summary>
        /// <param name="eventArgs">The Error</param>
        public static void SendError(PluginException eventArgs)
        {
            OnError?.Invoke(eventArgs);
        }

        /// <summary>
        /// Replaces the Current Error Handler.
        /// </summary>
        /// <param name="onError">The new Error Handler</param>
        public static void RegisterErrorHandler(PluginEvents.OnErrorEvent onError)
        {
            OnError = onError;
        }

        /// <summary>
        /// OnLog Event Delegate
        /// </summary>
        public static event PluginEvents.LogMessageEvent OnLog = DefaultLogHandler;

        /// <summary>
        /// Writes a Log through the PluginSystem OnLog Handler
        /// </summary>
        /// <param name="args">The Log to Write</param>
        public static void SendLog(LogMessageEventArgs args)
        {
            OnLog?.Invoke(args);
        }

        /// <summary>
        /// Helper Function that Creates a Divider in the Log
        /// </summary>
        public static void SendLogDivider()
        {
            SendLog("______________________________________________________");
        }

        /// <summary>
        /// On Clean Temp Folder Event is called before the System is deleting all content in the Internal Temp Folder and all Plugin Temp Folders.
        /// </summary>
        public static event PluginEvents.OnCleanTempFolderEvent OnCleanTempFolder;

        /// <summary>
        /// After Clean Temp Folder Event is called after the System has deleted all content in the Internal Temp Folder and all Plugin Temp Folders.
        /// </summary>
        public static event PluginEvents.AfterCleanTempFolderEvent AfterCleanTempFolder;

        /// <summary>
        /// On Initialized Event is called after all Initializations Finished.
        /// </summary>
        public static event PluginEvents.OnInitializedEvent OnInitialized;

        /// <summary>
        /// On Register Host Event is called before a Host gets Registered/Plugins for this Host are getting loaded.
        /// </summary>
        public static event PluginEvents.OnRegisterHostEvent OnRegisterHost;

        /// <summary>
        /// After Register Host Event is called after a Host gets Registered/Plugins for this Host were loaded.
        /// </summary>
        public static event PluginEvents.AfterRegisterHostEvent AfterRegisterHost;

        /// <summary>
        /// On UnRegister Host Event is called before a Host gets UnRegistered/Plugins for this Host are getting unloaded.
        /// </summary>
        public static event PluginEvents.OnUnRegisterHostEvent OnUnRegisterHost;

        /// <summary>
        /// After UnRegister Host Event is called after a Host gets UnRegistered/Plugins for this Host were unloaded.
        /// </summary>
        public static event PluginEvents.AfterUnRegisterHostEvent AfterUnRegisterHost;

        /// <summary>
        /// On Add Plugin Event is called before a Plugin gets added to a Host.
        /// </summary>
        public static event PluginEvents.OnAddPluginEvent OnAddPlugin;

        /// <summary>
        /// After Add Plugin Event is called after a Plugin got added to a Host.
        /// </summary>
        public static event PluginEvents.AfterAddPluginEvent AfterAddPlugin;

        /// <summary>
        /// On Remove Plugin Event is called before a Plugin gets removed from a Host.
        /// </summary>
        public static event PluginEvents.OnRemovePluginEvent OnRemovePlugin;

        /// <summary>
        /// After Remove Plugin Event is called after a Plugin got removed from a Host.
        /// </summary>
        public static event PluginEvents.AfterRemovePluginEvent AfterRemovePlugin;

        /// <summary>
        /// On Add Package Event is called before a Package gets added to the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.OnAddPackageEvent OnAddPackage;

        /// <summary>
        /// On Add Package Pointer Loaded Event is called before a Package gets added to the Plugin Folder Structure but the Pointer was successfully loaded.
        /// </summary>
        public static event PluginEvents.OnAddPackagePointerLoadedEvent OnAddPackagePointerLoaded;

        /// <summary>
        /// After Add Package Event is called after a Package got added to the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.AfterAddPackageEvent AfterAddPackage;

        /// <summary>
        /// On Install Package Event is called before a Package gets installed into the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.OnInstallPackageEvent OnInstallPackage;

        /// <summary>
        /// After Install Package Event is called after a Package got installed into the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.AfterInstallPackageEvent AfterInstallPackage;

        /// <summary>
        /// On Remove Package Event is called before a Package is removed from the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.OnRemovePackageEvent OnRemovePackage;

        /// <summary>
        /// After Remove Package Event is called after a Package was removed from the Plugin Folder Structure.
        /// </summary>
        public static event PluginEvents.AfterRemovePackageEvent AfterRemovePackage;

        /// <summary>
        /// On Activate Package Event is called before a Package is Activated.
        /// </summary>
        public static event PluginEvents.OnActivatePackageEvent OnActivatePackage;

        /// <summary>
        /// After Activate Package Event is called after a Package was Activated.
        /// </summary>
        public static event PluginEvents.AfterActivatePackageEvent AfterActivatePackage;

        /// <summary>
        /// On Deactivate Package Event is called before a Package is Deactivated.
        /// </summary>
        public static event PluginEvents.OnDeactivatePackageEvent OnDeactivatePackage;

        /// <summary>
        /// After Deactivate Package Event is called after a Package was Deactivated.
        /// </summary>
        public static event PluginEvents.AfterDeactivatePackageEvent AfterDeactivatePackage;

        #endregion

        #region Loading and Unloading a Host

        /// <summary>
        /// Loads all compatible Plugins that are installed in the system.
        /// </summary>
        /// <param name="host">The Host that the Plugins need to be Compatible to</param>
        public static void LoadPlugins(IPluginHost host, bool addPlugins = true)
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }

            bool contains = LoadedPlugins.ContainsKey(host);

            if (contains && !addPlugins)
            {
                return;
            }

            if (!contains)
            {
                RegisterHostEventArgs args = new RegisterHostEventArgs(host, true);
                OnRegisterHost?.Invoke(args);
                if (args.Cancel)
                {
                    return;
                }

                LoadedPlugins.Add(host, new List<IPlugin>());
            }


            if (addPlugins)
            {
                SendLog($"Adding Plugins for {host.GetType().Name}");

                List<PluginAssemblyPointer> ptrs = LoadFromList(PluginPaths.PluginListFile, host);
                LoadOrder.SortList(ptrs);

                ptrs.ForEach(AddFromLoaderResult);
                SendLog($"Added Plugins from {ptrs.Count} Packages");
            }


            AfterRegisterHost?.Invoke(new RegisterHostEventArgs(host, false));
            SendLogDivider();
        }

        /// <summary>
        /// Unloads all Plugins from the Host.
        /// </summary>
        /// <param name="host">The Host should be UnLoaded.</param>
        public static void UnloadPlugins(IPluginHost host)
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }


            if (!LoadedPlugins.ContainsKey(host))
            {
                return;
            }

            UnRegisterHostEventArgs args = new UnRegisterHostEventArgs(host, true);
            OnUnRegisterHost?.Invoke(args);
            if (args.Cancel)
            {
                return;
            }

            if (LoadedPlugins[host] == null)
            {
                return;
            }

            int plginCount = LoadedPlugins[host].Count;
            SendLog($"Removing {LoadedPlugins[host].Count} Plugins from {host.GetType().Name}");
            for (int i = LoadedPlugins[host].Count - 1; i >= 0; i--)
            {
                IPlugin plugin = LoadedPlugins[host][i];
                RemovePlugin(host, plugin);
            }

            LoadedPlugins[host].Clear();
            LoadedPlugins.Remove(host);
            AfterUnRegisterHost?.Invoke(new UnRegisterHostEventArgs(host, false));
            SendLog($"Removed {plginCount} Plugins from {host.GetType().Name}");
            SendLogDivider();
        }

        #endregion

        #region Adding Removing a Plugin from a host

        /// <summary>
        /// Adds a Plugin to the Host specified in the PluginPointer Data
        /// </summary>
        /// <param name="plugin">Plugin to Add</param>
        /// <param name="data">The Plugin Pointer</param>
        public static void AddPlugin(IPlugin plugin, PluginAssemblyPointer data)
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }

            if (!LoadedPlugins.ContainsKey(data.Host))
            {
                LoadPlugins(data.Host, false);
            }

            if (!LoadedPlugins[data.Host].Contains(plugin))
            {
                if (!data.Host.IsAllowedPlugin(plugin))
                {
                    return;
                }

                AddPluginEventArgs args = new AddPluginEventArgs(data, plugin, true);
                OnAddPlugin?.Invoke(args);
                if (args.Cancel)
                {
                    return;
                }


                SendLog($"Adding {plugin.GetType().Name} to {data.Host.GetType().Name}");
                if (plugin.HasIO)
                    data.EnsureDirectoriesExist();

                PointerMap[plugin] = data;
                LoadedPlugins[data.Host].Add(plugin);
                plugin.OnLoad(data);
                data.Host.OnPluginLoad(plugin, data);
                ProcessAttributes(plugin, data);

                AfterAddPlugin?.Invoke(new AddPluginEventArgs(data, plugin, false));
            }
        }

        private static void ProcessAttributes(IPlugin plugin, PluginAssemblyPointer ptr)
        {
            Dictionary<MemberInfo, Attribute[]> attribs =
                plugin.GetMemberAttributes<Attribute>(
                                                      true,
                                                      BindingFlags.Instance |
                                                      BindingFlags.Public |
                                                      BindingFlags.NonPublic
                                                     );
            foreach (KeyValuePair<MemberInfo, Attribute[]> keyValuePair in attribs)
            {
                keyValuePair.Value.ToList().ForEach(x => AttributeManager.Handle(plugin, ptr, keyValuePair.Key, x));
            }
        }

        
        public static bool IsPackageActivated(string name)
        {
            List<string> list = ListHelper.LoadList(PluginPaths.PluginListFile).ToList();
            return list.Select(x=>new BasePluginPointer(x)).Any(x=>x.PluginName == name);
        }


        public static bool IsActivated(IPlugin plugin)
        {
            return IsActivated(PointerMap[plugin].PluginName);
        }

        public static bool IsActivated(string name)
        {
            List<string> list = ListHelper.LoadList(PluginPaths.LoadOrderListFile).ToList();
            return list.Contains(name);
        }

        public static void SilentActivate(string name)
        {
            List<string> list = ListHelper.LoadList(PluginPaths.LoadOrderListFile).ToList();
            list.Add(name);
            ListHelper.SaveList(PluginPaths.LoadOrderListFile, list.Distinct().ToArray());
            ActivatePackage(name);
        }

        public static void SilentDeactivate(string name)
        {
            List<string> list = ListHelper.LoadList(PluginPaths.LoadOrderListFile).ToList();
            list.Remove(name);
            ListHelper.SaveList(PluginPaths.LoadOrderListFile, list.Distinct().ToArray());
            DeactivatePackage(name);
        }

        public static void SilentActivate(IPlugin plugin)
        {
            SilentActivate(PointerMap[plugin].PluginName);
        }

        public static void SilentDeactivate(IPlugin plugin)
        {
            SilentDeactivate(PointerMap[plugin].PluginName);
        }

        /// <summary>
        /// Removes a Plugin from a Host
        /// </summary>
        /// <param name="host">Target Host</param>
        /// <param name="plugin">Plugin to Remove</param>
        public static void RemovePlugin(IPluginHost host, IPlugin plugin)
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }


            if (LoadedPlugins.ContainsKey(host) && LoadedPlugins[host] != null && LoadedPlugins[host].Contains(plugin))
            {
                RemovePluginEventArgs args = new RemovePluginEventArgs(host, plugin, true);
                OnRemovePlugin?.Invoke(args);
                if (args.Cancel)
                {
                    return;
                }

                SendLog($"Removing {plugin.GetType().Name} from {host.GetType().Name}");
                plugin.OnUnload();
                host.OnPluginUnload(plugin);
                LoadedPlugins[host].Remove(plugin);
                AfterRemovePlugin?.Invoke(new RemovePluginEventArgs(host, plugin, false));
            }
        }

        #endregion

        #region Adding Removing a Plugin Package

        /// <summary>
        /// Adds a Package to the Plugin System
        /// </summary>
        /// <param name="file">Package Input Path</param>
        /// <param name="name">When Loaded successfully contains the Name of the Loaded plugin</param>
        /// <returns>True if the Adding was Successful</returns>
        public static bool AddPackage(string file, out string name)
        {
            if (!IsInitialized)
            {
                throw new Exception("Can not use the plugin System when its not initialized.");
            }

            name = null;
            SendLogDivider();
            SendLog("Adding File: " + file);


            if (PluginPacker.CanLoad(file))
            {
                AddPackageEventArgs<string> args = new AddPackageEventArgs<string>(file, true);
                OnAddPackage?.Invoke(args);
                if (args.Cancel)
                {
                    return false;
                }

                string tempDir = Path.Combine(
                                              PluginPaths.GetSystemProcessTempDirectory("Install"),
                                              Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                                             ); //TODO: Get temp dir for unpacking
                Directory.CreateDirectory(tempDir);

                //TODO: If the package is already installed Write Backup to PluginDir/backup before loading the new package

                SendLog("Trying to Load File Format: " + Path.GetFileName(file));
                PluginPacker.Unpack(file, tempDir);

                //TODO: Try load Package Data/Plugin Data
                if (PackageDataManager.CanLoad(tempDir))
                {
                    SendLog("Trying to Load Data Format: " + Path.GetFileName(tempDir));
                    BasePluginPointer ptr = PackageDataManager.LoadData(tempDir);
                    if (ptr != null)
                    {
                        name = ptr.PluginName;
                        ptr.EnsureDirectoriesExist();

                        AddPackageEventArgs<BasePluginPointer> ptrArgs =
                            new AddPackageEventArgs<BasePluginPointer>(ptr, true);
                        OnAddPackagePointerLoaded?.Invoke(ptrArgs);
                        if (ptrArgs.Cancel)
                        {
                            return false;
                        }

                        List<string> installedPackages = ListHelper.LoadList(PluginPaths.GlobalPluginListFile).ToList();
                        string newPackage = ptr.ToKeyPair();
                        bool isNew = !installedPackages.Contains(newPackage);
                        if (isNew)
                        {
                            installedPackages.Add(newPackage);
                            ListHelper.SaveList(PluginPaths.GlobalPluginListFile, installedPackages.ToArray());
                        }

                        //TODO: Check if the Install would overwrite things.
                        //TODO: Check if the files that are overwritten are in use.
                        //TODO: Make a system that takes instructions from a file at start up to "complete" installations
                        InstallPackageEventArgs iargs = new InstallPackageEventArgs(isNew, ptr, tempDir, true);
                        OnInstallPackage?.Invoke(iargs);
                        if (iargs.Cancel)
                        {
                            return false;
                        }

                        PackageDataManager.Install(ptr, tempDir);
                        AfterInstallPackage?.Invoke(new InstallPackageEventArgs(isNew, ptr, tempDir, false));

                        AfterAddPackage?.Invoke(new AfterAddPackageEventArgs(ptr));
                        return true;
                    }

                    SendError(new PackageDataException("File Corrupt", file));
                }
                else
                {
                    SendError(new PackageDataException("Unable to find a Data Serializer", file, null));
                }
            }
            else
            {
                SendError(new PackerException("Unable to find a Packer", file, null));
            }

            return false;
        }

        /// <summary>
        /// Removes a Package from the Plugin System
        /// </summary>
        /// <param name="ptr">Pointer Pointing to the Package that should be removed.</param>
        /// <param name="keepArchive">When set to false will also delete the backup archive.</param>
        public static void RemovePackage(BasePluginPointer ptr, bool keepArchive = true)
        {
            //TODO: Check if the Package is in the Installed List. Then we need to wait until the program restarted.

            List<string> installedPackages = ListHelper.LoadList(PluginPaths.GlobalPluginListFile).ToList();
            string package = ptr.ToKeyPair();
            if (installedPackages.Contains(package))
            {
                RemovePackageEventArgs args = new RemovePackageEventArgs(ptr, keepArchive, true);
                OnRemovePackage?.Invoke(args);
                if (args.Cancel)
                {
                    return;
                }

                keepArchive = args.KeepArchive;

                installedPackages.Remove(package);
                ListHelper.SaveList(PluginPaths.GlobalPluginListFile, installedPackages.ToArray());
                AfterRemovePackage?.Invoke(new RemovePackageEventArgs(ptr, keepArchive, false));
            }

            PluginPaths.RemovePluginPackageFromDirectoryStructure(ptr, keepArchive);
        }

        /// <summary>
        /// Activates a Package in the Plugin System.
        /// </summary>
        /// <param name="packageName">The Package to be activated.</param>
        /// <param name="addToExistingHosts">When True will add all plugins in the Package to Compatible Loaded Hosts.</param>
        public static void ActivatePackage(string packageName, bool addToExistingHosts = false)
        {
            List<BasePluginPointer> globalPackages =
                ListHelper.LoadList(PluginPaths.GlobalPluginListFile).Select(x => new BasePluginPointer(x)).ToList();
            BasePluginPointer packageKey = globalPackages.FirstOrDefault(x => x.PluginName == packageName);
            if (packageKey != null)
            {
                List<string> installedPackages = ListHelper.LoadList(PluginPaths.PluginListFile).ToList();
                string key = packageKey.ToKeyPair();

                if (!installedPackages.Contains(key))
                {
                    ActivatePackageEventArgs args = new ActivatePackageEventArgs(packageKey, true);
                    OnActivatePackage?.Invoke(args);
                    if (args.Cancel)
                    {
                        return;
                    }

                    List<BasePluginPointer> ptrs = installedPackages.Select(x => new BasePluginPointer(x)).ToList();

                    installedPackages.Add(key);

                    ListHelper.SaveList(PluginPaths.PluginListFile, installedPackages.ToArray());
                    if (addToExistingHosts)
                    {
                        foreach (KeyValuePair<IPluginHost, List<IPlugin>> loadedPlugin in LoadedPlugins)
                        {
                            PluginAssemblyPointer ptr = new PluginAssemblyPointer(
                                                                                  packageKey.PluginName,
                                                                                  packageKey.PluginFile,
                                                                                  packageKey.PluginOrigin,
                                                                                  loadedPlugin.Key
                                                                                 );
                            PluginLoader.AddPluginsFromLoaderResult(ptr);
                        }
                    }

                    //UnloadPlugins(PluginHost);

                    LoadPlugins(PluginHost);

                    AfterActivatePackage?.Invoke(new ActivatePackageEventArgs(packageKey, false));
                }
            }
        }

        public static void DeactivatePackage(IPlugin plugin)
        {
            if(!PointerMap.ContainsKey(plugin))
                throw new ArgumentException("Internal data Corrupt.");
            DeactivatePackage(PointerMap[plugin].PluginName);
        }

        /// <summary>
        /// Deactivates a Package so its plugins are not considered when searching for compatible plugins for a Host.
        /// </summary>
        /// <param name="packageName">Name of package that should be deactivated.</param>
        public static void DeactivatePackage(string packageName)
        {
            List<string> list = ListHelper.LoadList(PluginPaths.PluginListFile).ToList();
            List<BasePluginPointer> installedPackages = list.Select(x => new BasePluginPointer(x)).ToList();
            BasePluginPointer ptr = installedPackages.FirstOrDefault(x => x.PluginName == packageName);
            if (ptr != null)
            {
                DeactivatePackageEventArgs args = new DeactivatePackageEventArgs(ptr, true);
                OnDeactivatePackage?.Invoke(args);
                if (args.Cancel)
                {
                    return;
                }

                list.Remove(ptr.ToKeyPair());
                ListHelper.SaveList(PluginPaths.PluginListFile, list.ToArray());
                AfterDeactivatePackage?.Invoke(new DeactivatePackageEventArgs(ptr, false));
            }
        }

        #endregion

        #region GetPluginQueries

        /// <summary>
        /// Returns all Hosts that are Assignable to T
        /// </summary>
        /// <typeparam name="T">"Minimum" Type</typeparam>
        /// <returns>List of Hosts that are of Type T</returns>
        public static List<T> GetHosts<T>() where T : IPluginHost
        {
            List<T> ret = new List<T>();
            foreach (KeyValuePair<IPluginHost, List<IPlugin>> loadedPlugin in LoadedPlugins)
            {
                if (loadedPlugin.Key is T t)
                {
                    ret.Add(t);
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns all Plugins that are Assignable to T
        /// </summary>
        /// <typeparam name="T">"Minimum" Type</typeparam>
        /// <returns>List of Plugins that are of Type T</returns>
        public static List<T> GetPlugins<T>() where T : IPlugin
        {
            List<T> ret = new List<T>();
            foreach (KeyValuePair<IPluginHost, List<IPlugin>> loadedPlugin in LoadedPlugins)
            {
                ret.AddRange(loadedPlugin.Value.Where(x => x is T).Cast<T>());
            }

            return ret;
        }

        /// <summary>
        /// Returns all Plugins that are Assignable to T and are Added to a specific host.
        /// </summary>
        /// <typeparam name="T">"Minimum" Type</typeparam>
        /// <param name="host">The Host</param>
        /// <returns>List of Plugins that are of Type T</returns>
        public static List<T> GetPlugins<T>(IPluginHost host) where T : IPlugin
        {
            if (!LoadedPlugins.ContainsKey(host))
            {
                return new List<T>();
            }

            return LoadedPlugins[host].Where(x => x is T).Cast<T>().ToList();
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Ensures that the Directory structure of a Pointer exists.
        /// Loads all Plugins From a Pointer.
        /// </summary>
        /// <param name="data">The Pointer to load.</param>
        private static void AddFromLoaderResult(PluginAssemblyPointer data)
        {
            data.EnsureDirectoriesExist();
            PluginLoader.AddPluginsFromLoaderResult(data);
        }


        /// <summary>
        /// Returns a List of Plugin Pointer based on a List path and a Host.
        /// </summary>
        /// <param name="path">The Path of the List File</param>
        /// <param name="host">Host Instance</param>
        /// <returns></returns>
        private static List<PluginAssemblyPointer> LoadFromList(string path, IPluginHost host)
        {
            return ListHelper.LoadList(path).Where(x => !string.IsNullOrEmpty(x))
                             .Select(x => new PluginAssemblyPointer(x, host)).ToList();
        }

        #endregion

    }
}