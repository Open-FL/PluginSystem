using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;

namespace PluginSystem.StartupActions
{
    public static class ActionRunner
    {

        static ActionRunner()
        {
            AddActionInstruction(new ActivatePackageAction());
            AddActionInstruction(new AddPackageAction());
            AddActionInstruction(new DeactivatePackageAction());
            AddActionInstruction(new RemovePackageAction());
            AddActionInstruction(new AddActivatePackageAction());
        }

        public const string ADD_PACKAGE_ACTION = "add-package";
        public const string REMOVE_PACKAGE_ACTION = "remove-package";
        public const string ACTIVATE_PACKAGE_ACTION = "activate-package";
        public const string DEACTIVATE_PACKAGE_ACTION = "deactivate-package";
        public const string ADD_ACTIVATE_PACKAGE_ACTION = "add-activate-package";
        private static readonly List<StartupAction> Actions = new List<StartupAction>();

        public static void AddActionInstruction(StartupAction action)
        {
            if (Actions.Any(x => x.GetType() == action.GetType())) return;
            Actions.Add(action);
        }

        public static void AddActionToStartup(string line)
        {
            if (!File.Exists(PluginPaths.InternalStartupInstructionPath))
                File.WriteAllLines(PluginPaths.InternalStartupInstructionPath, new string[0]);
            List<string> lines = File.ReadAllLines(PluginPaths.InternalStartupInstructionPath).ToList();
            if (lines.Contains(line)) return;
            lines.Add(line);
            File.WriteAllLines(PluginPaths.InternalStartupInstructionPath, lines);
        }

        internal static void RunActions()
        {
            string[] lines = File.ReadAllLines(PluginPaths.InternalStartupInstructionPath);
            List<(string key, string[] content)> instructions = lines
                                                                .Select(
                                                                        x =>
                                                                            (x.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0],
                                                                             x.Split(
                                                                                     new[] { ' ' },
                                                                                     StringSplitOptions
                                                                                         .RemoveEmptyEntries
                                                                                    ).Skip(1).ToArray())
                                                                       ).ToList();
            foreach ((string key, string[] content) instruction in instructions)
            {
                StartupAction action = Actions.FirstOrDefault(x => x.ActionName == instruction.key);
                action?.RunAction(instruction.content);
            }
            File.Delete(PluginPaths.InternalStartupInstructionPath);
        }




    }
}