using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 可导出零件
    /// </summary>
    internal sealed class Exportable : IExportable
    {
        /// <summary>
        /// 注册零件委托
        /// </summary>
        private readonly Action<ContainerBuilder> _register;

        /// <summary>
        /// 初始化可导出零件
        /// </summary>
        /// <param name="register"></param>
        private Exportable(Action<ContainerBuilder> register) => _register = register;

        /// <summary>
        /// 注册零件
        /// </summary>
        public void Register(ContainerBuilder builder) => _register(builder);


#pragma warning disable IDE0011
        /// <summary>
        /// 根据类型的接口规则返回可导出零件
        /// </summary>
        public static IEnumerable<IExportable> ByInterface(Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.Name.EndsWith("AutofacPart", StringComparison.Ordinal))
                {
                    yield return new Exportable(b => b.RegisterTypes(type).As(@interface));
                }
                else
                {
                    var contract = Contract.Export(@interface, false);
                    if (contract.Valid)
                    {
                        if (contract.Name != null)
                        {
                            yield return new Exportable(b => b.RegisterTypes(type).Named(contract.Name, contract.Type ?? @interface));
                        }
                        else
                        {
                            yield return new Exportable(b => b.RegisterTypes(type).As(contract.Type ?? @interface));
                        }
                    }
                }
            }
        }

#pragma warning restore IDE0011

        /// <summary>
        /// 根据父类的特性返回可导出零件
        /// </summary>
        public static IEnumerable<IExportable> ByBaseType(Type type)
        {
            var realType = type;
            while (true)
            {
                type = type?.BaseType;
                if (type == typeof(object) || type == null)
                {
                    yield break;
                }
                var contract = Contract.Export(type, true);
                if (contract.Valid)
                {
                    if (contract.Name != null)
                    {
                        yield return new Exportable(b => b.RegisterTypes(realType).Named(contract.Name, contract.Type ?? type));
                    }
                    else
                    {
                        yield return new Exportable(b => b.RegisterTypes(realType).As(contract.Type ?? type));
                    }
                }
            }
        }

        /// <summary>
        /// 根据特性规则返回可导出零件
        /// </summary>
        public static IEnumerable<IExportable> ByAttribute(Type type)
        {
            var contract = Contract.Export(type, false);
            if (contract.Valid)
            {
                if (contract.Name != null)
                {
                    yield return new Exportable(b => b.RegisterTypes(type).Named(contract.Name, contract.Type ?? type));
                }
                else
                {
                    yield return new Exportable(b => b.RegisterTypes(type).As(contract.Type ?? type));
                }
            }
        }

        /// <summary>
        /// 根据特性规则返回可导出零件
        /// </summary>
        public static IEnumerable<IExportable> Method(MethodInfo method)
        {
            var contract = Contract.Export(method);
            if (contract.Valid)
            {
                yield return new Exportable(b => b.RegisterInstance(method).Named(contract.Name ?? method.Name, typeof(MethodInfo)));
            }
        }
    }
}
