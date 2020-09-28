using PluginSystem.Core;

namespace PluginSystem.StartupActions
{
    public class ActivatePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.ACTIVATE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.ActivatePackage(parameter[0]);
        }

    }
}