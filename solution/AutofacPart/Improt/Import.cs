using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Autofac;
using System.Collections;

namespace blqw.Autofac
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class Import : IImprot
    {
        private readonly TryResolveHandler _handler;

        private delegate bool TryResolveHandler(IContainer container, out object value);

        private Import(TryResolveHandler handler) => _handler = handler;

        public bool TryResolve(IContainer container, out object value) => _handler(container, out value);


        public static IImprot ByProperty(PropertyInfo property)
        {
            var contract = Contract.Import(property);
            if (!contract.Valid)
            {
                return null;
            }
            if (contract.Name == null)
            {
                return new Import((IContainer container, out object value) =>
                {
                    if (contract.IsMany)
                    {
                        var type = typeof(IEnumerable<>).MakeGenericType(contract.Type);
                        if (container.TryResolve(type, out value) == false)
                        {
                            return false;
                        }
                        try
                        {
                            value = Units.ConvertToCollection((IEnumerable)value, contract.Type, property.PropertyType);
                            return true;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                    return container.TryResolve(contract.Type, out value);
                });
            }

            return new Import((IContainer container, out object value) =>
            {
                if (contract.IsMany)
                {
                    var type = typeof(IEnumerable<>).MakeGenericType(contract.Type);
                    if (container.TryResolveNamed(contract.Name, type, out value) == false)
                    {
                        return false;
                    }
                    try
                    {
                        value = Units.ConvertToCollection((IEnumerable)value, contract.Type, property.PropertyType);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
                return container.TryResolveNamed(contract.Name, contract.Type, out value);
            });
        }

        public static IImprot ByField(FieldInfo field)
        {
            return null;
        }

    }
}
