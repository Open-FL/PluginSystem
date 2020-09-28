using PluginSystem.Core;

namespace PluginSystem.StartupActions
{
    public class AddActivatePackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.ADD_ACTIVATE_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.AddPackage(parameter[0], out string name);
            PluginManager.ActivatePackage(name);
        }

    }
}