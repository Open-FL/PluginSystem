using System;

using PluginSystem.Core.Pointer;

namespace PluginSystem.Updating
{
    public interface IPluginUpdateChecker
    {

        bool CanCheck(BasePluginPointer ptr);

        void CheckAndUpdate(BasePluginPointer ptr, Func<string, string, bool> updateDialog, Action<string, int, int> setStatus);

    }
}