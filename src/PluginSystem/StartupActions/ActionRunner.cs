using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PluginSystem.Core;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;

namespace PluginSystem.StartupActions
{
    public abstract class StartupAction
    {

        public abstract string ActionName { get; }

        public abstract void RunAction(string[] parameter);

    }

    public class AddPackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.ADD_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.AddPackage(parameter[0], out string name);
        }

    }

    public class RemovePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.REMOVE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.RemovePackage(PluginManager.GetPluginFromName(parameter[0]));
        }

    }
    public class ActivatePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.ACTIVATE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.ActivatePackage(parameter[0]);
        }

    }

    public class DeactivatePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.DEACTIVATE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.DeactivatePackage(parameter[0]);
        }

    }

    public static class ActionRunner
    {

        public const string ADD_PACKAGE_ACTION = "add-package";
        public const string REMOVE_PACKAGE_ACTION = "remove-package";
        public const string ACTIVATE_PACKAGE_ACTION = "activate-package";
        public const string DEACTIVATE_PACKAGE_ACTION = "deactivate-package";
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