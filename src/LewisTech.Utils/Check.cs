using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace LewisTech.Utils
{
    [DebuggerStepThrough]
    public static class Check
    {
        [ContractAnnotation("value:null => halt;")]
        public static void NotNull<T>([NoEnumeration] T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (ReferenceEquals(value, null))
            {
                NotEmpty(parameterName, "parameterName");

                throw new ArgumentNullException(parameterName);
            }

            //return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>([NoEnumeration] T value, [InvokerParameterName] [NotNull] string parameterName, [NotNull] string propertyName)
        {
            if (ReferenceEquals(value, null))
            {
                NotEmpty(parameterName, "parameterName");
                NotEmpty(propertyName, "propertyName");

                throw new ArgumentException(Strings.ArgumentPropertyNull(propertyName, parameterName));
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static IReadOnlyList<T> NotEmpty<T>(IReadOnlyList<T> value, [InvokerParameterName] [NotNull] string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                NotEmpty(parameterName, "parameterName");

                throw new ArgumentException(Strings.CollectionArgumentIsEmpty(parameterName));
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            Exception e = null;
            if (ReferenceEquals(value, null))
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsEmpty(parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");

                throw e;
            }

            return value;
        }

        [ContractAnnotation("value:null => halt")]
        public static string NotEmpty(string value, [InvokerParameterName] [NotNull] string parameterName, [NotNull] string propertyName)
        {
            Exception e = null;
            if (ReferenceEquals(value, null))
            {
                e = new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsEmpty(propertyName, parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");
                NotEmpty(propertyName, "propertyName");

                throw e;
            }

            return value;
        }

        public static string NullButNotEmpty(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (!ReferenceEquals(value, null) && value.Length == 0)
            {
                NotEmpty(parameterName, "parameterName");

                throw new ArgumentException(Strings.ArgumentIsEmpty(parameterName));
            }

            return value;
        }

        public static IReadOnlyList<T> HasNoNulls<T>(IReadOnlyList<T> value, [InvokerParameterName] [NotNull] string parameterName) where T : class
        {
            NotNull(value, parameterName);

            if (value.Any(e => e == null))
            {
                NotEmpty(parameterName, "parameterName");

                throw new ArgumentException(parameterName);
            }

            return value;
        }

        public static T IsDefined<T>(T value, [InvokerParameterName] [NotNull] string parameterName) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                NotEmpty(parameterName, "parameterName");

                throw new ArgumentException(Strings.InvalidEnumValue(parameterName, typeof(T)));
            }

            return value;
        }

        public static void NotZero(int value, [InvokerParameterName] [NotNull] string parameterName)
        {
            Exception e = null;
            if (value == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsZero(parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");

                throw e;
            }
        }

        public static void NotZero(int value, [InvokerParameterName] [NotNull] string parameterName, [NotNull] string propertyName)
        {
            Exception e = null;
            if (value == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsZero(propertyName, parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");
                NotEmpty(propertyName, "propertyName");

                throw e;
            }
        }

        public static void NotZero(decimal value, [InvokerParameterName] [NotNull] string parameterName)
        {
            Exception e = null;
            if (value == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsZero(parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");

                throw e;
            }
        }

        public static void NotZero(decimal value, [InvokerParameterName] [NotNull] string parameterName, [NotNull] string propertyName)
        {
            Exception e = null;
            if (value == 0)
            {
                e = new ArgumentException(Strings.ArgumentIsZero(propertyName, parameterName));
            }

            if (e != null)
            {
                NotEmpty(parameterName, "parameterName");
                NotEmpty(propertyName, "propertyName");

                throw e;
            }
        }

        [ContractAnnotation("condition:false => halt")]
        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new AssertFailedException("Failed Assertion: " + message);
            }
        }

        internal static class Strings
        {
            /// <summary>
            /// The string argument '{argumentName}' cannot be empty.
            /// </summary>
            public static string ArgumentIsEmpty([CanBeNull] object argumentName)
            {
                return string.Format("The string argument '{0}' cannot be empty.", argumentName);
            }

            /// <summary>
            /// The string argument '{argumentName}' cannot be empty.
            /// </summary>
            public static string ArgumentIsEmpty([CanBeNull] string propertyName, object argumentName)
            {
                return string.Format("The string property '{0}' on argument '{1}' cannot be empty.", propertyName, argumentName);
            }

            /// <summary>
            /// The string argument '{argumentName}' cannot be empty.
            /// </summary>
            public static string InvalidEnumValue([CanBeNull] object argumentName, [CanBeNull] object enumType)
            {
                return string.Format("The value provided for argument '{0}' must be a valid value of enum type '{1}'.", argumentName, enumType);
            }

            /// <summary>
            /// The property '{property}' of the argument '{argument}' cannot be null.
            /// </summary>
            public static string ArgumentPropertyNull([CanBeNull] object property, [CanBeNull] object argument)
            {
                return string.Format("The property '{0}' of the argument '{1}' cannot be null.", property, argument);
            }

            /// <summary>
            /// The collection argument '{argumentName}' must contain at least one element.
            /// </summary>
            public static string CollectionArgumentIsEmpty([CanBeNull] object argumentName)
            {
                return string.Format("The collection argument '{0}' must contain at least one element.", argumentName);
            }

            /// <summary>
            /// The string argument '{argumentName}' cannot be empty.
            /// </summary>
            public static string ArgumentIsZero([CanBeNull] object argumentName)
            {
                return string.Format("The argument '{0}' cannot be zero.", argumentName);
            }

            /// <summary>
            /// The property '{property}' of the argument '{argument}' cannot be null.
            /// </summary>
            public static string ArgumentIsZero([CanBeNull] object property, [CanBeNull] object argument)
            {
                return string.Format("The property '{0}' of the argument '{1}' cannot be zero.", property, argument);
            }
        }
    }

    internal class AssertFailedException : Exception
    {
        public AssertFailedException(string message)
            : base(message)
        {
        }
    }
}