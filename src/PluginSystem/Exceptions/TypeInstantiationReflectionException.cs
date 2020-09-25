using System;

namespace PluginSystem.Exceptions
{
    public class TypeInstantiationReflectionException : PluginException
    {

        public readonly Type PluginType;

        public TypeInstantiationReflectionException(string message, Type pluginType, Exception inner) :
            base(message, inner)
        {
            PluginType = pluginType;
        }

        public TypeInstantiationReflectionException(string message, Type pluginType) : base(message)
        {
            PluginType = pluginType;
        }


        public override string ToString()
        {
            return $"{Message} < On Type: {PluginType.Name}({PluginType.AssemblyQualifiedName})";
        }

    }
}