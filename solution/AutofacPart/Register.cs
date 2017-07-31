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

        public static IEnumerable<IExport> ByInterface(Type type)
        {
            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.Name.EndsWith("AutofacPart", StringComparison.Ordinal))
                {
                    yield return new Export(b => b.RegisterTypes(type).As(@interface));
                }
            }
        }

        public void Register(ContainerBuilder builder) => _to(builder);
    }
}
