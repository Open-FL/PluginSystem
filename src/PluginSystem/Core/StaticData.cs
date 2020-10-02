using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PluginSystem.Core
{
    public static class StaticData
    {

        public static string InitStartupInstrsName = "startup-action";

        /// <summary>
        ///     Separator used for Splitting a single String into a Key and a Value
        /// </summary>
        public static char KeyPairSeparator { get; } = '|';

        //Extensions:

        /// <summary>
        ///     Extension used to Store Plugin Version information Data
        /// </summary>
        public static string PluginVersionExtension { get; } = ".pv";

        /// <summary>
        ///     Extension used to store Lists.
        /// </summary>
        public static string PluginListExtension { get; } = ".ls";

        //Internal Folder Names:
        /// <summary>
        ///     The Name of the Folder where files will be downloaded.
        /// </summary>
        public static string DownloadFolder { get; } = "downloads";

        /// <summary>
        ///     The Name of the Folder which will be used to create temp files.
        /// </summary>
        public static string TempFolder { get; } = "temp";

        /// <summary>
        ///     The Name of the Folder which will be used to store archive files.
        /// </summary>
        public static string ArchiveFolder { get; } = "archives";


        /// <summary>
        ///     The Name of the Folder which will be used to store config files.
        /// </summary>
        public static string ConfigFolder { get; } = "config";


        /// <summary>
        ///     The Name of the List File which will be used to store Activated packages.
        /// </summary>
        public static string PluginListName { get; } = "installed-packages";

        /// <summary>
        ///     The Name of the List File which will be used to store all packages.
        /// </summary>
        public static string GlobalPluginListName { get; } = "global-packages";

        /// <summary>
        ///     The Name of the List File which will be used to store Activated Init Plugins.
        /// </summary>
        public static string InitPluginListName { get; } = "init-packages";

        /// <summary>
        ///     The Name of the List File which will be used to store the Load order of the Plugins
        /// </summary>
        public static string LoadOrderListName { get; } = "load-order";

        //Plugin Folder Names
        /// <summary>
        ///     The Name of the Folder that contains the Binary Data of a Plugin/Package
        /// </summary>
        public static string PluginBinFolder { get; } = "bin";

        /// <summary>
        ///     The Name of the Folder that contains the Config Data of a Plugin/Package
        /// </summary>
        public static string PluginConfigFolder { get; } = "config";

        /// <summary>
        ///     Returns the State of the StaticData object.
        /// </summary>
        /// <returns></returns>
        public static string GetState()
        {
            string data = "";
            State[] states = GetStates();
            foreach (State state in states)
            {
                string value = state.Getter.Invoke(null, null).ToString();
                string name = state.Name;
                data += $"{name}={value}\n";
            }

            return data;
        }

        /// <summary>
        ///     Sets the Variables in StaticData to the data specified.
        /// </summary>
        /// <param name="data">Data Content</param>
        internal static void SetState(string data)
        {
            string[] dataValues = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> map = new Dictionary<string, string>();
            foreach (string dataValue in dataValues)
            {
                string[] content = dataValue.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (content.Length == 1)
                {
                    return;
                }

                map.Add(content[0], content[1]);
            }

            State[] states = GetStates().Where(x => map.ContainsKey(x.Name)).ToArray();
            foreach (State state in states)
            {
                object[] parameter = new object[1];
                if (state.Setter.GetParameters()[0].ParameterType == typeof(char))
                {
                    parameter[0] = map[state.Name][0];
                }
                else
                {
                    parameter[0] = map[state.Name];
                }

                state.Setter.Invoke(null, parameter);
            }
        }

        /// <summary>
        ///     Uses Reflection to get all properties of the StaticData class that are of type string or char
        /// </summary>
        /// <returns>State array</returns>
        private static State[] GetStates()
        {
            Type t = typeof(StaticData);
            return t.GetProperties().Where(x => x.PropertyType == typeof(char) || x.PropertyType == typeof(string))
                    .Select(x => new State(x)).ToArray();
        }

        /// <summary>
        ///     Struct used to Apply and Get the values to the static data
        /// </summary>
        private struct State
        {

            /// <summary>
            ///     Property Getter
            /// </summary>
            public readonly MethodInfo Getter;

            /// <summary>
            ///     Property Setter
            /// </summary>
            public readonly MethodInfo Setter;

            /// <summary>
            ///     Property Name
            /// </summary>
            public readonly string Name;

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="pi">Property Info of the Property that this state is based on</param>
            public State(PropertyInfo pi)
            {
                Name = pi.Name;
                Getter = pi.GetMethod;
                Setter = pi.SetMethod;
            }

        }

    }
}