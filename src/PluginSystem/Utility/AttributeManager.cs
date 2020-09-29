using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using PluginSystem.Core;
using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;

namespace PluginSystem.Utility
{
    public static class AttributeManager
    {

        public static readonly List<IAttributeHandler<Attribute>> Handlers = new List<IAttributeHandler<Attribute>>();

        /// <summary>
        ///     Count of Installed Packer Formats
        /// </summary>
        public static int FormatCount => Handlers.Count;

        /// <summary>
        ///     Adds a Packer Format to the system
        /// </summary>
        /// <param name="formatProvider">The format to be added.</param>
        public static void AddAttributeHandler(IAttributeHandler<Attribute> formatProvider)
        {
            if (Handlers.All(x => x.GetType() != formatProvider.GetType()))
            {
                PluginManager.SendLog($"Adding Attribute Handler: {formatProvider.GetType().Name}");
                Handlers.Add(formatProvider);
            }
        }

        public static void Handle<T>(IPlugin plugin, PluginAssemblyPointer ptr, MemberInfo info, T attribute)
            where T : Attribute
        {
            Handlers.ForEach(x => x.Handle(plugin, ptr, info, attribute));
        }

        /// <summary>
        ///     Removes a Packer Format from the system
        /// </summary>
        /// <param name="formatProvider">The format to be removed.</param>
        public static void RemoveAttributeHandler(IAttributeHandler<Attribute> formatProvider)
        {
            if (Handlers.Contains(formatProvider))
            {
                PluginManager.SendLog($"Removing Attribute Handler: {formatProvider.GetType().Name}");
                Handlers.Remove(formatProvider);
            }
        }

        private static List<Type> GetBaseTypes(Type type)
        {
            List<Type> ret = new List<Type>();
            Type current = type;
            while (current != null)
            {
                ret.Add(current);

                Type newb = current.BaseType;
                current = newb == current ? null : newb;
            }

            return ret;
        }


        public static Dictionary<MemberInfo, T[]> GetMemberAttributes<T>(
            this object plugin, bool inherit = true, BindingFlags flags = BindingFlags.Default) where T : Attribute
        {
            Dictionary<MemberInfo, T[]> infos = new Dictionary<MemberInfo, T[]>();
            MemberInfo[] members = plugin.GetType().GetMembers(flags);

            foreach (MemberInfo memberInfo in members)
            {
                List<T> keys = memberInfo.GetCustomAttributes<T>(inherit).ToList();
                if (memberInfo is MethodInfo mi && mi.IsVirtual && inherit)
                {
                    MethodInfo baseM = mi.GetBaseDefinition();
                    while (baseM != null)
                    {
                        keys.AddRange(baseM.GetCustomAttributes<T>(inherit).ToList());

                        MethodInfo newb = baseM.GetBaseDefinition();
                        baseM = newb == baseM ? null : newb;
                    }
                }

                if (keys.Count != 0)
                {
                    infos.Add(memberInfo, keys.ToArray());
                }
            }

            return infos;
        }

    }
}