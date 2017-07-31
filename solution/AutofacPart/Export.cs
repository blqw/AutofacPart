using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace blqw.Autofac
{
    internal sealed class Export : IExport
    {
        private readonly Action<ContainerBuilder> _to;

        public Export(Action<ContainerBuilder> to) => _to = to;

        public static bool IsInstantiable(Type type)
        {
            if (type == null || type.IsAbstract || !type.IsClass)
            {
                return false;
            }
            return !type.IsGenericType || type.IsConstructedGenericType;
        }


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

        public void Register(ContainerBuilder builder) => _to(builder);
    }
}
