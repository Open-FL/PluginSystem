using PluginSystem.Core;

namespace PluginSystem.StartupActions
{
    public class AddPackageAction : StartupAction
    {

        public override string ActionName => ActionRunner.ADD_PACKAGE_ACTION;

        public override void RunAction(string[] parameter)
        {
            PluginManager.AddPackage(parameter[0], out string name);
        }

    }
}