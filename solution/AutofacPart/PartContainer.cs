using System.Linq;
using Autofac.Util;
using System.Reflection;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    /// <summary>
    /// 零件容器
    /// </summary>
    public sealed class PartContainer
    {
        static PartContainer() => Rebuild();

        static IContainer _container;
        public static void Rebuild()
        {
            var builder = new ContainerBuilder();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(AssemblyExtensions.GetLoadableTypes);

            foreach (var type in types)
            {
                foreach (var register in Export.ByInterface(type))
                {
                    register.Register(builder);
                }
            }

            _container = builder.Build();
        }

        public static void PropertiesAutowired(object instance)
        {

        }
    }
}
