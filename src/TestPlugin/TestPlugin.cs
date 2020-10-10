using System.Windows.Forms;

namespace TestPlugin
{
    public class TestPlugin : APlugin<Form1>
    {

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            MessageBox.Show("Test Plugin loaded.");
        }

    }
}