using PluginSystem.Core;

namespace PluginSystem.StartupActions
{
    public class DeactivatePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.DEACTIVATE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.DeactivatePackage(parameter[0]);
        }

    }
}