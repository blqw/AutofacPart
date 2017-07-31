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
                if (Export.IsInstantiable(type))
                {
                    foreach (var register in Export.ByInterface(type))
                    {
                        register.Register(builder);
                    }

                    foreach (var register in Export.ByAttribute(type))
                    {
                        register.Register(builder);
                    }
                }
            }

            _container = builder.Build();
        }

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
                if (!p.CanWrite || p.GetSetMethod()?.IsStatic != isStatic)
                {
                    continue;
                }
                var contract = Contract.Import(p);
                if (!contract.Valid)
                {
                    continue;
                }
                var result = contract.Name == null
                            ? _container.TryResolve(contract.Type, out var value)
                            : _container.TryResolveNamed(contract.Name, contract.Type, out value);
                if (result)
                {
                    p.SetValue(instance, value);
                }
            }
        }
    }
}
