using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace blqw.Autofac
{
    public sealed class PartServiceProvider : IServiceProvider
    {
        private readonly IContainer _container;

        public PartServiceProvider(IContainer container) 
            => _container = container ?? throw new ArgumentNullException(nameof(container));

        public object GetService(Type serviceType)
        {
            if (serviceType.IsAssignableTo<Delegate>())
            {
                var service = ResolveMethodHelper.CreateDelegate(_container, serviceType);
                if (service != null)
                {
                    return service;
                }
            }

            return _container.Resolve(serviceType);
        }
    }
}
