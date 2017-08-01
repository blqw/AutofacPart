using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    /// <summary>
    /// 导出零件
    /// </summary>
    internal sealed class Export : IExport
    {
        /// <summary>
        /// 注册零件委托
        /// </summary>
        private readonly Action<ContainerBuilder> _register;

        /// <summary>
        /// 初始化导出零件
        /// </summary>
        /// <param name="register"></param>
        public Export(Action<ContainerBuilder> register) => _register = register;

        /// <summary>
        /// 注册零件
        /// </summary>
        public void Register(ContainerBuilder builder) => _register(builder);
        

#pragma warning disable IDE0011
        public static IEnumerable<IExport> ByInterface(Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.Name.EndsWith("AutofacPart", StringComparison.Ordinal))
                {
                    yield return new Export(b => b.RegisterTypes(type).As(@interface));
                }
                else foreach (var export in ByAttribute(@interface))
                {
                    yield return export;
                }
            }
        }
#pragma warning restore IDE0011

        public static IEnumerable<IExport> ByAttribute(Type type)
        {
            var contract = Contract.Export(type);
            if (contract.Valid)
            {
                if (contract.Name != null)
                {
                    yield return new Export(b => b.RegisterTypes(type).Named(contract.Name, contract.Type));
                }
                else
                {
                    yield return new Export(b => b.RegisterTypes(type).As(contract.Type));
                }
            }
        }

    }
}
