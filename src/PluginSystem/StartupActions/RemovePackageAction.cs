using PluginSystem.Core;

namespace PluginSystem.StartupActions
{
    public class RemovePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.REMOVE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.RemovePackage(PluginManager.GetPluginFromName(parameter[0]));
        }

    }
}