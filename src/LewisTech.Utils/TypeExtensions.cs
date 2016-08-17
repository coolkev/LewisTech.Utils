using System;
using System.Linq;
using System.Text;

namespace LewisTech.Utils
{
    public static class TypeExtensions
    {

        public static string DisplayName(this Type type)
        {
            var name = type.Name;
            if (!type.IsGenericType)
            {
                return name;
            }

            var sb = new StringBuilder();

            sb.Append(name.Substring(0, name.IndexOf('`')));
            sb.Append("<");
            sb.Append(string.Join(", ", type.GetGenericArguments()
                                            .Select(t => t.DisplayName())));
            sb.Append(">");

            var output = sb.ToString();

            return output;
        }

    }
}
