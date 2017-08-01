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
    public static class PartContainer
    {
        static PartContainer() => Rebuild();

        /// <summary>
        /// autofac 容器
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// 重新编译 (一般只有在动态加载程序集后才需要调用)
        /// </summary>
        public static void Rebuild()
        {
            var builder = new ContainerBuilder();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(AssemblyExtensions.GetLoadableTypes);

            foreach (var type in types)
            {
                if (Units.IsInstantiable(type))
                {
                    foreach (var export in Exportable.ByInterface(type))
                    {
                        export.Register(builder);
                    }

                    foreach (var export in Exportable.ByAttribute(type))
                    {
                        export.Register(builder);
                    }
                }
            }

            _container = builder.Build();
        }

        /// <summary>
        /// 填充属性和字段
        /// </summary>
        /// <param name="instance">待填充属性的对象</param>
        public static void Fill(object instance)
        {
            if (instance == null)
            {
                return;
            }
            TypeInfo type;
            bool isStatic;
            switch (instance)
            {
                case TypeInfo t:
                    type = t;
                    isStatic = true;
                    instance = null;
                    break;
                case Type t:
                    type = t.GetTypeInfo();
                    isStatic = true;
                    instance = null;
                    break;
                default:
                    type = instance.GetType().GetTypeInfo();
                    isStatic = false;
                    break;
            }

            foreach (var p in type.GetRuntimeProperties())
            {
                if (!p.CanWrite || p.GetSetMethod(true)?.IsStatic != isStatic)
                {
                    continue;
                }
                var import = Improtable.ByProperty(p);
                if (import != null && import.TryResolve(_container, out var value))
                {
                    p.SetValue(instance, value);
                }
            }

            foreach (var f in type.GetRuntimeFields())
            {
                if (f.IsStatic != isStatic)
                {
                    continue;
                }
                var import = Improtable.ByField(f);
                if (import != null && import.TryResolve(_container, out var value))
                {
                    f.SetValue(instance, value);
                }
            }
        }

        /// <summary>
        /// 获取零件
        /// </summary>
        public static T Get<T>() => _container.Resolve<T>();

        /// <summary>
        /// 获取零件
        /// </summary>
        public static object Get(Type contractType) => _container.Resolve(contractType);

        /// <summary>
        /// 获取零件
        /// </summary>
        public static T Get<T>(string contractName) => _container.ResolveNamed<T>(contractName);

        /// <summary>
        /// 获取零件
        /// </summary>
        public static object Get(string contractName, Type contractType) => _container.ResolveNamed(contractName, contractType);
    }
}
