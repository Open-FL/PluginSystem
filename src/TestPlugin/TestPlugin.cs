using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PluginSystem.Core.Pointer;
using PluginSystem.Utility;

using TestApplication;

namespace TestPlugin
{
    public class TestPlugin: APlugin<Form1>
    {

        public override void OnLoad(PluginAssemblyPointer ptr)
        {
            base.OnLoad(ptr);
            MessageBox.Show("Test Plugin loaded.");
        }

    }
}
