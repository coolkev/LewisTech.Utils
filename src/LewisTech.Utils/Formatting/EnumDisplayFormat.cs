using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LewisTech.Utils.Formatting
{
   public static class EnumDisplayFormat
    {
        public static string DisplayName<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }

            return EnumDisplayCache<T>.Values[enumerationValue];
        }

        /// <summary>
        /// Turns a CamelCase word into separate words
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ExpandCaps(this string str)
        {
            return Regex.Replace(str, "([a-z])([A-Z])", "$1 $2");
        }

        private static class EnumDisplayCache<T>
        {
            public static readonly Dictionary<T, string> Values = GetValues();

            private static Dictionary<T, string> GetValues()
            {
                Type type = typeof(T);
                if (!type.IsEnum)
                {
                    throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
                }

                var values = type.GetEnumValues().Cast<T>().ToDictionary(
                    m => m, 
                    m =>
                        {
                            var name = type.GetEnumName(m);

                            MemberInfo[] memberInfo = type.GetMember(name);
                            if (memberInfo != null && memberInfo.Length > 0)
                            {
                                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

                                if (attrs != null && attrs.Length > 0)
                                {
                                    //Pull out the description value
                                    return ((DisplayAttribute)attrs[0]).Name;
                                }
                            }
                            return name.ExpandCaps();
                        });

                return values;
            }
        }
    }
}
