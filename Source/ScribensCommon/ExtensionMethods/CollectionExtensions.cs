using System.Linq;
using System.Collections.Specialized;

namespace PluginScribens.Common.ExtensionMethods
{
    public static class CollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection nameValueCollection)
        {
            var items = from key in nameValueCollection.AllKeys
                        from value in nameValueCollection.GetValues(key)
                        select $"{key}={value.Encode()}";

            return string.Join("&", items);
        }
    }
}
