using System;
using System.Reflection;

using PluginSystem.Core.Interfaces;
using PluginSystem.Core.Pointer;

namespace PluginSystem.Utility
{
    public interface IAttributeHandler<in T> where T : Attribute
    {

        void Handle(IPlugin plugin, PluginAssemblyPointer ptr, MemberInfo mi, T obj);

    }
}