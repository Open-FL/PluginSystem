﻿using System;
using System.Collections.Generic;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;

namespace PluginSystem.Updating
{
    public static class UpdateManager
    {
        private static readonly List<IPluginUpdateChecker> Checker = new List<IPluginUpdateChecker>
                                                                     {
                                                                         new FileEqualityUpdateChecker()
                                                                     };

        public static void CheckAndUpdate(BasePluginPointer ptr, Func<string, string, bool> updateDialog)
        {
            if (Checker.All(x => !x.CanCheck(ptr))) return;

            PluginManager.SendLog("Updating Plugin: " + ptr.PluginName);
            Checker.First(x => x.CanCheck(ptr)).CheckAndUpdate(ptr, updateDialog);
        }

    }
}