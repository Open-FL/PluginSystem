using PluginSystem.Core.Pointer;
using PluginSystem.Events.Args;
using PluginSystem.Exceptions;

namespace PluginSystem.Events
{
    public static class PluginEvents
    {

        public delegate void AfterActivatePackageEvent(ActivatePackageEventArgs eventArgs);

        public delegate void AfterAddPackageEvent(AfterAddPackageEventArgs eventArgs);

        public delegate void AfterAddPluginEvent(AddPluginEventArgs eventArgs);

        public delegate void AfterCleanTempFolderEvent();

        public delegate void AfterDeactivatePackageEvent(DeactivatePackageEventArgs eventArgs);

        public delegate void AfterInstallPackageEvent(InstallPackageEventArgs eventArgs);

        public delegate void AfterRegisterHostEvent(RegisterHostEventArgs eventArgs);

        public delegate void AfterRemovePackageEvent(RemovePackageEventArgs eventArgs);

        public delegate void AfterRemovePluginEvent(RemovePluginEventArgs eventArgs);

        public delegate void AfterUnRegisterHostEvent(UnRegisterHostEventArgs eventArgs);

        public delegate void LogMessageEvent(LogMessageEventArgs eventArgs);

        public delegate void OnActivatePackageEvent(ActivatePackageEventArgs eventArgs);

        public delegate void OnAddPackageEvent(AddPackageEventArgs<string> eventArgs);

        public delegate void OnAddPackagePointerLoadedEvent(AddPackageEventArgs<BasePluginPointer> eventArgs);

        public delegate void OnAddPluginEvent(AddPluginEventArgs eventArgs);

        public delegate void OnCleanTempFolderEvent(CleanTempFolderEventArgs eventArgs);

        public delegate void OnDeactivatePackageEvent(DeactivatePackageEventArgs eventArgs);

        public delegate void OnErrorEvent(PluginException exception);

        public delegate void OnInitializedEvent();

        public delegate void OnInstallPackageEvent(InstallPackageEventArgs eventArgs);

        public delegate void OnRegisterHostEvent(RegisterHostEventArgs eventArgs);

        public delegate void OnRemovePackageEvent(RemovePackageEventArgs eventArgs);

        public delegate void OnRemovePluginEvent(RemovePluginEventArgs eventArgs);

        public delegate void OnUnRegisterHostEvent(UnRegisterHostEventArgs eventArgs);

    }
}