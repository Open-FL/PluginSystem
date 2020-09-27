using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PluginSystem.Core;
using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;
using PluginSystem.FileSystem;

namespace TestApplication
{
    public partial class Form1 : Form, IPluginHost
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnInstallPackage_Click(object sender, EventArgs e)
        {
            PluginManager.AddPackage(tbPackageOrigin.Text, out string name);
            PluginManager.ActivatePackage(name);
            MessageBox.Show("Added Package: " + (name ?? "NULL"));
        }

        public bool IsAllowedPlugin(IPlugin plugin)
        {
            return true;
        }

        public void OnPluginLoad(IPlugin plugin, BasePluginPointer ptr)
        {
        }

        public void OnPluginUnload(IPlugin plugin)
        {
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Task t = new Task(() => PluginManager.Initialize(
                                                             Path.Combine(PluginPaths.EntryDirectory, "data"),
                                                             "internal",
                                                             "plugins",
                                                             (msg, title) =>
                                                                 MessageBox.Show(
                                                                                 msg,
                                                                                 title,
                                                                                 MessageBoxButtons.YesNo,
                                                                                 MessageBoxIcon.Question
                                                                                ) ==
                                                                 DialogResult.Yes,
                                                             (status, current, max) =>
                                                             {
                                                                 lblStatus.Text = status;
                                                                 pbProgress.Maximum = max;
                                                                 pbProgress.Value = current;
                                                             }
                                                            ,
                                                             Path.Combine(PluginPaths.EntryDirectory, "static-data.sd")
                                                            ));

            t.Start();
            while (!t.IsCompleted)
            {
                Application.DoEvents();
            }

            if (t.IsFaulted)
            {
                MessageBox.Show(
                                t.Exception.InnerException.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                               );
            }
            else
            {
                PluginManager.LoadPlugins(this);
            }
        }
    }
}
