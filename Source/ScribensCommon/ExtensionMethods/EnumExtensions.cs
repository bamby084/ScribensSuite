using System;

namespace PluginScribens.Common.ExtensionMethods
{
    public static class EnumExtensions
    {
        public static T GetAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            var memberInfos = enumValue.GetType().GetMember(enumValue.ToString());
            var attributes = memberInfos[0].GetCustomAttributes(typeof(T), false);
            if (attributes.Length > 0)
                return (T) attributes[0];

            return null;
        }
    }
}
