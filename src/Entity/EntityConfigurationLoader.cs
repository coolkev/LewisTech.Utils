using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;

namespace LewisTech.Utils.Entity
{
    public static class EntityConfigurationLoader
    {

        public static void LoadConfigurationsFromAssembly(DbModelBuilder modelBuilder, Assembly assembly)
        {

            // from: http://stackoverflow.com/questions/4383024/reflection-to-build-list-of-entitytypeconfiguration-for-entity-framework-cpt5
            // Load all EntityTypeConfiguration<T> from current assembly and add to configurations
            var mapTypes = from t in assembly.GetTypes()
                           where t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)
                           select t;

            foreach (var mapType in mapTypes)
            {
                // note: "dynamic" is a nifty piece of work which bypasses compile time type checking... (urgh??)
                //       Check out: http://msdn.microsoft.com/en-us/library/vstudio/dd264741%28v=vs.100%29.aspx
                dynamic mapInstance = Activator.CreateInstance(mapType);
                modelBuilder.Configurations.Add(mapInstance);
            }

        }
    }
}
