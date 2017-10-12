using System.Diagnostics;
using Autofac.Core;
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
        /// 重新编译
        /// </summary>
        /// <remarks> 一般只有在动态加载程序集后才需要调用, 暂不公开 </remarks>
        private static void Rebuild()
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

                    foreach (var export in Exportable.ByBaseType(type))
                    {
                        export.Register(builder);
                    }
                }
                if (!type.IsInterface)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (!method.IsAbstract)
                        {
                            foreach (var export in Exportable.Method(method))
                            {
                                export.Register(builder);
                            }
                        }
                    }
                }
            }

            _container = builder.Build();
        }

        public static void Noop() { }

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


        /// <summary>
        /// 执行零件方法获取返回值
        /// </summary>
        public static T Invoke<T>(string contractName, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(contractName))
            {
                throw new NullReferenceException(nameof(contractName));
            }

            var method = _container.ResolveNamed<MethodInfo>(contractName);
            if (method == null)
            {
                throw new EntryPointNotFoundException("零件不存在");
            }

            return (T)method.Invoke(null, args);

        }


        /// <summary>
        /// 执行零件方法获取返回值
        /// </summary>
        public static T Invoke<T>(string contractName, IEnumerable<object> args)
                        => Invoke<T>(contractName, args as object[] ?? args.ToArray());






        //public static object CreateInstance(Type type)
        //{
        //    if (type == null)
        //    {
        //        throw new ArgumentNullException(nameof(type));
        //    }
        //    var ctors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic);
        //    if (ctors.Length == 0)
        //    {
        //        throw new InvalidOperationException("没有可用的构造函数");
        //    }
        //    foreach (var ctor in ctors)
        //    {
        //        var instance = CreateInstance(ctor);
        //        if (instance != null)
        //        {
        //            return instance;
        //        }
        //    }
        //    return null;
        //}

        //public static object CreateInstance(ConstructorInfo ctor)
        //{
        //    if (ctor == null)
        //    {
        //        throw new ArgumentNullException(nameof(ctor));
        //    }

        //    var param = ctor.GetParameters();
        //    if (param.Length == 0)
        //    {
        //        var instance = ctor.Invoke(Array.Empty<object>());
        //        Fill(instance);
        //        return instance;
        //    }



        //}

        //public static object[] GetArguments(ParameterInfo[] parameters, Func<ParameterInfo, object> defaultFactory = null)
        //{
        //    if (parameters == null)
        //    {
        //        throw new ArgumentNullException(nameof(parameters));
        //    }
        //    var length = parameters.Length;
        //    if (length == 0)
        //    {
        //        return Array.Empty<object>();
        //    }
        //    var args = new object[length];
        //    for (var i = 0; i < length; i++)
        //    {
        //        if (true)
        //        {

        //        }
        //    }


        //}
    }
}
