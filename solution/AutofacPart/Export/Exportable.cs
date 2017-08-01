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
                else foreach (var export in ByAttribute(@interface))
                {
                    yield return export;
                }
            }
        }
#pragma warning restore IDE0011

        /// <summary>
        /// 根据特性规则返回可导出零件
        /// </summary>
        public static IEnumerable<IExportable> ByAttribute(Type type)
        {
            var contract = Contract.Export(type);
            if (contract.Valid)
            {
                if (contract.Name != null)
                {
                    yield return new Exportable(b => b.RegisterTypes(type).Named(contract.Name, contract.Type));
                }
                else
                {
                    yield return new Exportable(b => b.RegisterTypes(type).As(contract.Type));
                }
            }
        }

    }
}
