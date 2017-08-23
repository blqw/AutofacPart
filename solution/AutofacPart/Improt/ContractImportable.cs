using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using System.Collections;
using System.Linq;
using System.Reflection;
using Autofac.Features.Metadata;

namespace blqw.Autofac
{
    internal sealed class ContractImportable : IImprotable
    {
        public ContractImportable(Contract contract)
        {
            Contract = contract;
        }

        public Contract Contract { get; }

        public bool TryResolve(IContainer container, out object part)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            if (Contract.Valid == false)
            {
                throw new InvalidOperationException("契约无效!");
            }

            var resolver = (IResolveHelper)Activator.CreateInstance(typeof(ResolveHelper<>).MakeGenericType(Contract.ContractType ?? Contract.PartType));

            if (resolver.TryResolve(container, Contract, out part))
            {
                return true;
            }

            if (typeof(Delegate).IsAssignableFrom(Contract.PartType)) //委托
            {
                resolver = new ResolveMethodHelper(Contract.PartType);
                if (resolver.TryResolve(container, Contract, out part))
                {
                    return true;
                }
            }
            
            return false;
        }


        

        
        
    }
}
